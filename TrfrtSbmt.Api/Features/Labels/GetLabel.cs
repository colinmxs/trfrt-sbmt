using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Labels
{
    public class GetLabel
    {
        public record GetLabelQuery(string LabelId, int PageSize = 20, string? PaginationKey = null) : IRequest<GetLabelResult> 
        {
            internal Dictionary<string, AttributeValue>? ExclusiveStartKey => GetLastEvaluatedKey(PaginationKey);
            private static Dictionary<string, AttributeValue>? GetLastEvaluatedKey(string? paginationKey)
            {
                if (string.IsNullOrEmpty(paginationKey)) return null;
                paginationKey = Encoding.UTF8.GetString(Convert.FromBase64String(paginationKey));
                return new Dictionary<string, AttributeValue>
                {
                    [nameof(BaseEntity.PartitionKey)] = new AttributeValue { S = paginationKey?.Split('|')[0] },
                    [nameof(BaseEntity.SortKey)] = new AttributeValue { S = paginationKey?.Split('|')[1] }
                };
            }
        }

        public record GetLabelResult(IEnumerable<ViewModel> Submissions, int PageSize, string? PaginationKey)
        {
            public GetLabelResult(IEnumerable<ViewModel> submissions, int pageSize, Dictionary<string, AttributeValue>? lastEvaluatedKey) :
                this(submissions, pageSize, GetPaginationKey(lastEvaluatedKey))
            { }

            private static string? GetPaginationKey(Dictionary<string, AttributeValue>? lastEvaluatedKey)
            {
                if (lastEvaluatedKey == null) return null;
                if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
                if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;

                return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{lastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{lastEvaluatedKey[nameof(BaseEntity.SortKey)].S}"));
            }
        }

        public record ViewModel(string Id, string Name, string State, string City, string Image)
        {
            public ViewModel(Submission s) : this(s.EntityId, s.Name, s.State, s.City, s.Image) { }
        }

        public class QueryHandler : IRequestHandler<GetLabelQuery, GetLabelResult>
        {
            private readonly IAmazonDynamoDB _db;
            private readonly AppSettings _settings;

            public QueryHandler(IAmazonDynamoDB db, AppSettings settings)
            {
                _db = db;
                _settings = settings;
            }
            public async Task<GetLabelResult> Handle(GetLabelQuery request, CancellationToken cancellationToken)
            {
                var vms = new List<ViewModel>();
                var submissionLabelsResult = await new DynamoDbQueries.Query(_db, _settings).ExecuteAsync(request.LabelId, request.ExclusiveStartKey);
                foreach (var item in submissionLabelsResult.Items.Select(i => new SubmissionLabel(i)))
                {
                    vms.Add(new ViewModel(item.SubmissionEntityId, item.Name, item.State, item.City, item.Image));
                }

                return new GetLabelResult(vms, request.PageSize, submissionLabelsResult.LastEvaluatedKey);
            }            
        }
    }
}
