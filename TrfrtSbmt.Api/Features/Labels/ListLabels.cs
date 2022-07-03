namespace TrfrtSbmt.Api.Features.Labels;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;

public class ListLabels
{
    public record ListLabelsQuery(string FestivalId, int PageSize = 20, string? PaginationKey = null) : IRequest<ListLabelsResult> 
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

    public record ListLabelsResult(string FestivalId, IEnumerable<LabelViewModel> Labels, int PageSize, string? PaginationKey) 
    {
        public ListLabelsResult(string festivalId, IEnumerable<Label> labels, int pageSize, Dictionary<string, AttributeValue>? lastEvaluatedKey) :
        this(festivalId, labels.Select(l => new LabelViewModel(l.EntityId, l.Name)), pageSize, GetPaginationKey(lastEvaluatedKey))
        { }

        private static string? GetPaginationKey(Dictionary<string, AttributeValue>? lastEvaluatedKey)
        {
            if (lastEvaluatedKey == null) return null;
            if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
            if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{lastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{lastEvaluatedKey[nameof(BaseEntity.SortKey)].S}"));
        }
    }

    public class QueryHandler : IRequestHandler<ListLabelsQuery, ListLabelsResult>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        public async Task<ListLabelsResult> Handle(ListLabelsQuery request, CancellationToken cancellationToken)
        {
            var pkSymbol = ":partitionKey";
            var skSymbol = ":sortKey";
            var queryResult = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol} and begins_with({nameof(BaseEntity.SortKey)}, {skSymbol})",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = request.FestivalId },
                    [skSymbol] = new AttributeValue { S = $"{nameof(Label)}-" }
                },
                Limit = request.PageSize,
                ExclusiveStartKey = request.ExclusiveStartKey

            });
            var labels = queryResult.Items.Select(i => new Label(i));
            return new ListLabelsResult(request.FestivalId, labels, request.PageSize, queryResult.LastEvaluatedKey);
        }
    }
}
