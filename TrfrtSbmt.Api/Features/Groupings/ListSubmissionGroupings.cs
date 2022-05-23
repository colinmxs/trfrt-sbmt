using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Linq;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Groupings;

public class ListSubmissionGroupings
{
    public record Query(bool ActiveOnly = false) : IRequest<Result>;
    
    public record Result(List<Model> Models)
    {
        public Result(List<Grouping> groupings) : this(groupings.Select(g => new Model(g)).ToList()) {}
    }

    public record Model(string GroupingName, List<string> SubGroupings, string Guidelines, DateTime StartDateTime, DateTime EndDateTime)
    {
        public Model(Grouping grouping) : this(grouping.GroupingName, grouping.SubGroupings, grouping.Guidelines, grouping.StartDateTime, grouping.EndDateTime) {}
    }

    public class QueryHandler : IRequestHandler<Query, Result>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var pkSymbol = ":partitionKey";
            var groupings = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                IndexName = BaseEntity.Gsi1,
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue {S = nameof(Grouping)}
                }
            });
            if(request.ActiveOnly)
            {
                groupings.Items = groupings.Items.Where(g => DateTime.Parse(g[nameof(Grouping.StartDateTime)].S) <= DateTime.Now && DateTime.Now <= DateTime.Parse(g[nameof(Grouping.EndDateTime)].S)).ToList();
            }
            return new Result(groupings.Items.Select(g => new Grouping(g)).ToList());
        }
    }
}