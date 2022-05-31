namespace TrfrtSbmt.Api.Features.Forts;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;

public class ListForts
{
    public record ListFortsQuery(string FestivalId, int PageSize = 20, string? PaginationKey = null) : IRequest<ListFortsResult>;

    public record ListFortsResult(string FestivalId, IEnumerable<FortViewModel> Forts, int PageSize, string? PaginationKey);

    public class QueryHandler : IRequestHandler<ListFortsQuery, ListFortsResult>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        public async Task<ListFortsResult> Handle(ListFortsQuery request, CancellationToken cancellationToken)
        {
            var pkSymbol = ":partitionKey";
            var skSymbol = ":sortKey";
            var queryResult = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol} and begins_with({nameof(BaseEntity.SortKey)}, {skSymbol})",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = request.FestivalId },
                    [skSymbol] = new AttributeValue { S = $"{nameof(Fort)}-"}
                },
                Limit = request.PageSize,
                ExclusiveStartKey = GetLastEvaluatedKey(request),

            });
            if (queryResult.Items == null) return new ListFortsResult(request.FestivalId, new List<FortViewModel>(), request.PageSize, null);
            var forts = queryResult.Items.Select(i => new Fort(i));

            string? paginationKey = GetPaginationKey(queryResult);
            return new ListFortsResult(request.FestivalId, forts.Select(f => new FortViewModel(f.EntityId, f.Name)), request.PageSize, paginationKey);
        }

        private static string? GetPaginationKey(QueryResponse queryResult)
        {
            if (queryResult.LastEvaluatedKey == null) return null;
            if (!queryResult.LastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
            if (!queryResult.LastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;

            return $"{queryResult.LastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{queryResult.LastEvaluatedKey[nameof(BaseEntity.SortKey)].S}";
        }

        private Dictionary<string, AttributeValue> GetLastEvaluatedKey(ListFortsQuery request)
        {
            if (string.IsNullOrEmpty(request.PaginationKey)) return null;
            return new Dictionary<string, AttributeValue> 
            {
                [nameof(BaseEntity.PartitionKey)] = new AttributeValue { S = request.PaginationKey?.Split('|')[0] },
                [nameof(BaseEntity.SortKey)] = new AttributeValue { S = request.PaginationKey?.Split('|')[1] }
            };
        }
    }
}
