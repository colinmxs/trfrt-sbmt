namespace TrfrtSbmt.Api.Features.Submissions;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Security.Claims;
using System.Text;
using TrfrtSbmt.Api.DataModels;
public class ListSubmissions
{
    public record ListSubmissionsQuery(string FestivalId, string FortId, int PageSize = 20, string? PaginationKey = null) : IRequest<ListSubmissionsResult>
    {
        internal Dictionary<string, AttributeValue>? ExclusiveStartKey => GetLastEvaluatedKey(PaginationKey);
        private Dictionary<string, AttributeValue>? GetLastEvaluatedKey(string? paginationKey)
        {
            if (string.IsNullOrEmpty(paginationKey)) return null;
            paginationKey = Encoding.UTF8.GetString(Convert.FromBase64String(paginationKey));
            return new Dictionary<string, AttributeValue>
            {
                [nameof(BaseEntity.PartitionKey)] = new AttributeValue { S = paginationKey?.Split('|')[0] },
                [nameof(BaseEntity.SortKey)] = new AttributeValue { S = paginationKey?.Split('|')[1] },
                [nameof(Submission.SubmissionDate)] = new AttributeValue { S = paginationKey?.Split('|')[2] }
            };
        }
    }

    public record ListSubmissionsResult(string FestivalId, string FortId, List<ViewModel> Submissions, int PageSize = 20, string? PaginationKey = null)
    {
        public ListSubmissionsResult(string festivalId, string fortId, IEnumerable<Submission> submissions, int pageSize, Dictionary<string, AttributeValue>? lastEvaluatedKey) :
        this(festivalId, fortId, submissions.Select(s => new ViewModel(s)).ToList(), pageSize, GetPaginationKey(lastEvaluatedKey))
        { }
        private static string? GetPaginationKey(Dictionary<string, AttributeValue>? lastEvaluatedKey)
        {
            if (lastEvaluatedKey == null) return null;
            if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
            if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;
            if (!lastEvaluatedKey.ContainsKey(nameof(Submission.SubmissionDate))) return null;

            var paginationKey = $"{lastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{lastEvaluatedKey[nameof(BaseEntity.SortKey)].S}|{lastEvaluatedKey[nameof(Submission.SubmissionDate)].S}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(paginationKey));
        }
    }

    public record ViewModel(string Id, string Name, string State, string City, string Image)
    {
        public ViewModel(Submission s) : this(s.EntityId, s.Name, s.State, s.City, s.Image) { }
    }
    
    public class QueryHandler : IRequestHandler<ListSubmissionsQuery, ListSubmissionsResult>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;

        public QueryHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user)
        {
            _db = db;
            _settings = settings;
            _user = user;
        }
        public async Task<ListSubmissionsResult> Handle(ListSubmissionsQuery request, CancellationToken cancellationToken)
        {
            List<Submission> submissions;
            QueryResponse queryResult;
            if (_user.IsAdmin() || _user.IsVoter())
            {
                queryResult = await SubmissionDateIndexQuery(_db, _settings, request.FortId, request.PageSize, request.ExclusiveStartKey);
                if (queryResult.Items == null) return new ListSubmissionsResult(request.FestivalId, request.FortId, new List<ViewModel>(), request.PageSize, null);
                submissions = queryResult.Items.Select(i => new Submission(i)).ToList();
            }
            else
            {
                queryResult = await new DynamoDbQueries.CreatedByQuery(_db, _settings).ExecuteAsync(_user.Claims.Single(c => c.Type == "username").Value);
                submissions = queryResult.Items.Select(i => new Submission(i)).ToList();
            }

            return new ListSubmissionsResult(request.FestivalId, request.FortId, submissions, request.PageSize, queryResult.LastEvaluatedKey);
        }

        private static async Task<QueryResponse> SubmissionDateIndexQuery(IAmazonDynamoDB db, AppSettings settings, string id, int pageSize, Dictionary<string, AttributeValue>? exclusiveStartKey)
        {            
            var pkSymbol = ":partitionKey";
            return await db.QueryAsync(new QueryRequest(settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = id }
                },
                Limit = pageSize,
                ExclusiveStartKey = exclusiveStartKey,
                IndexName = "SubmissionDateIndex"
            });
        }
    }
}