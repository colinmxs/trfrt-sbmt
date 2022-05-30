namespace TrfrtSbmt.Api.Features.Festivals;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TrfrtSbmt.Api.DataModels;

public class ListFestivals
{
    /// <summary>
    /// Get festivals from the database.
    /// </summary>
    /// <param name="ActiveOnly">If true, only return festivals accepting submissions.</param>
    public record ListFestivalsQuery(bool ActiveOnly = false, int PageSize = 20, string? PaginationKey = null) : IRequest<ListFestivalsResult>;
    public record ListFestivalsResult(IEnumerable<FestivalViewModel> Festivals, int PageSize, string? PaginationKey);
    public record FestivalViewModel(string Id, string Name, string Guidelines, DateTime StartDateTime, DateTime EndDateTime)
    {
        public FestivalViewModel(Festival grouping) : this(grouping.EntityId, grouping.Name, grouping.Guidelines, grouping.StartDateTime, grouping.EndDateTime) { }
    }

    public class QueryHandler : IRequestHandler<ListFestivalsQuery, ListFestivalsResult>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        public async Task<ListFestivalsResult> Handle(ListFestivalsQuery request, CancellationToken cancellationToken)
        {
            var pkSymbol = ":partitionKey";
            var queryResult = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                IndexName = BaseEntity.Gsi1,
                KeyConditionExpression = $"{nameof(BaseEntity.SearchTerm)} = {pkSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = nameof(Festival).ToUpperInvariant() }
                },
                Limit = request.PageSize,
                ExclusiveStartKey = GetLastEvaluatedKey(request),

            });
            if (queryResult.Items == null) return new ListFestivalsResult(new List<FestivalViewModel>(), request.PageSize, null);
            var festivals = queryResult.Items.Select(i => new Festival(i));
            if (request.ActiveOnly)
            {
                festivals = festivals.Where(f => f.StartDateTime <= DateTime.UtcNow && DateTime.UtcNow <= f.EndDateTime).ToList();
            }

            return new ListFestivalsResult(festivals.Select(f => new FestivalViewModel(f)), request.PageSize, $"{queryResult.LastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{queryResult.LastEvaluatedKey[nameof(BaseEntity.SortKey)].S}");
        }

        private static Dictionary<string, AttributeValue>? GetLastEvaluatedKey(ListFestivalsQuery request)
        {
            return request.PaginationKey != null ? 
                new Dictionary<string, AttributeValue> 
                {
                    [nameof(BaseEntity.SearchTerm)] = new AttributeValue { S = nameof(Festival).ToUpperInvariant() },
                    [nameof(BaseEntity.PartitionKey)] = new AttributeValue { S = request.PaginationKey.Split('|')[0] },
                    [nameof(BaseEntity.SortKey)] = new AttributeValue { S = request.PaginationKey.Split('|')[1] },
                    [nameof(BaseEntity.EntityType)] = new AttributeValue { S = nameof(Festival) }
                } : null;
        }
    }
}