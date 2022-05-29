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
    public record Query(bool ActiveOnly = false) : IRequest<List<FestivalViewModel>>;    

    public record FestivalViewModel(string Id, string Name, string Guidelines, DateTime StartDateTime, DateTime EndDateTime)
    {
        public FestivalViewModel(Festival grouping) : this(grouping.EntityId, grouping.Name, grouping.Guidelines, grouping.StartDateTime, grouping.EndDateTime) { }
    }

    public class QueryHandler : IRequestHandler<Query, List<FestivalViewModel>>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        public async Task<List<FestivalViewModel>> Handle(Query request, CancellationToken cancellationToken)
        {
            var pkSymbol = ":partitionKey";
            var queryResult = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                IndexName = BaseEntity.Gsi1,
                KeyConditionExpression = $"{nameof(BaseEntity.SearchTerm)} = {pkSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = nameof(Festival) }
                }
            });
            var festivals = queryResult.Items.Select(i => new Festival(i));
            if (request.ActiveOnly)
            {
                festivals = festivals.Where(f => f.StartDateTime <= DateTime.UtcNow && DateTime.UtcNow <= f.EndDateTime).ToList();
            }

            return festivals.Select(f => new FestivalViewModel(f)).ToList();
        }
    }
}