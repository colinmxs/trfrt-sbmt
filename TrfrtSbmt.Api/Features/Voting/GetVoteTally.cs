using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text;
using TrfrtSbmt.Domain;

namespace TrfrtSbmt.Api.Features.Voting
{
    public class GetVoteTally
    {
        public enum Sort
        {
            HighToLow,
            LowToHigh
        }
        public record GetVoteTallyQuery(string FestivalId, string FortId, Sort Sort = Sort.HighToLow, int PageSize = 20, string? PaginationKey = null) : IRequest<VoteTallyResult> 
        {
            internal Dictionary<string, AttributeValue>? ExclusiveStartKey => GetLastEvaluatedKey(PaginationKey);
            private Dictionary<string, AttributeValue>? GetLastEvaluatedKey(string? paginationKey)
            {
                if (string.IsNullOrEmpty(paginationKey)) return null;
                paginationKey = Encoding.UTF8.GetString(Convert.FromBase64String(paginationKey));
                return new Dictionary<string, AttributeValue>
                {
                    [nameof(SubmissionRank.PartitionKey)] = new AttributeValue { S = paginationKey?.Split('|')[0] },
                    [nameof(SubmissionRank.FortId)] = new AttributeValue { S = paginationKey?.Split('|')[1] },
                    [nameof(SubmissionRank.SortKey)] = new AttributeValue { S = paginationKey?.Split('|')[2] },
                    [nameof(SubmissionRank.AverageScore)] = new AttributeValue { N = paginationKey?.Split('|')[3] }
                };
            }
        }
        public record VoteTallyResult(IEnumerable<VoteTallyViewModel> Results, int PageSize = 20, string? PaginationKey = null)
        {
            public VoteTallyResult(IEnumerable<SubmissionRank> results, int pageSize, Dictionary<string, AttributeValue>? lastEvaluatedKey) :
            this(results.Select(r => new VoteTallyViewModel(r)), pageSize, GetPaginationKey(lastEvaluatedKey))
            { }
            private static string? GetPaginationKey(Dictionary<string, AttributeValue>? lastEvaluatedKey)
            {
                if (lastEvaluatedKey == null) return null;
                if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
                if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;
                if (!lastEvaluatedKey.ContainsKey(nameof(SubmissionRank.FortId))) return null;
                if (!lastEvaluatedKey.ContainsKey(nameof(SubmissionRank.AverageScore))) return null;
                var paginationKey = $"{lastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{lastEvaluatedKey[nameof(SubmissionRank.FortId)].S}|{lastEvaluatedKey[nameof(BaseEntity.SortKey)].S}|{lastEvaluatedKey[nameof(SubmissionRank.AverageScore)].N}";
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(paginationKey));
            }
        }
        public record VoteTallyViewModel(string Id, string Name, string State, string City, string Country, string Image, string FortId, string AverageScore, string NumberOfVotes) 
        {
            public VoteTallyViewModel(SubmissionRank submissionRank) : this(submissionRank.SubmissionEntityId, submissionRank.Name, submissionRank.State, submissionRank.City, submissionRank.Country, submissionRank.Image, submissionRank.FortId, submissionRank.AverageScore, submissionRank.NumberOfVotes)
            { }
        }
        public class GetVoteTallyQueryHandler : IRequestHandler<GetVoteTallyQuery, VoteTallyResult>
        {
            private readonly IAmazonDynamoDB _db;
            private readonly AppSettings _settings;

            public GetVoteTallyQueryHandler(IAmazonDynamoDB dynamoDbClient, AppSettings settings)
            {
                _db = dynamoDbClient;
                _settings = settings;
            }
            
            public async Task<VoteTallyResult> Handle(GetVoteTallyQuery request, CancellationToken cancellationToken)
            {
                var queryResult = await new DynamoDbQueries.VoteTallyQuery(_db, _settings).ExecuteAsync(request.FortId, request.Sort == Sort.LowToHigh, request.PageSize, request.ExclusiveStartKey);
                var ranks = queryResult.Items.Select(i => new SubmissionRank(i)).ToList();
                return new VoteTallyResult(ranks, request.PageSize, queryResult.LastEvaluatedKey);
            }
        }
    }
}
