namespace TrfrtSbmt.Api.Features.Forts;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;

public class ListForts
{
    public record ListFortsQuery(string FestivalId, int PageSize = 20, string? PaginationKey = null) : IRequest<ListFortsResult> 
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
    };

    public record ListFortsResult(string FestivalId, IEnumerable<FortViewModel> Forts, int PageSize, string? PaginationKey)
    {
        public ListFortsResult(string festivalId, IEnumerable<Fort> forts, int pageSize, Dictionary<string, AttributeValue>? lastEvaluatedKey) :
        this(festivalId, forts.Select(f => new FortViewModel(f)), pageSize, GetPaginationKey(lastEvaluatedKey))
        { }

        private static string? GetPaginationKey(Dictionary<string, AttributeValue>? lastEvaluatedKey)
        {
            if (lastEvaluatedKey == null) return null;
            if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
            if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{lastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{lastEvaluatedKey[nameof(BaseEntity.SortKey)].S}"));
        }
    }

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
            var queryResult = await new DynamoDbQueries.BeginsWithQuery(_db, _settings).ExecuteAsync(request.FestivalId, $"{nameof(Fort)}-", request.PageSize, request.ExclusiveStartKey);            
            if (queryResult.Items == null) return new ListFortsResult(request.FestivalId, new List<FortViewModel>(), request.PageSize, null);
            var forts = queryResult.Items.Select(i => new Fort(i));
            return new ListFortsResult(request.FestivalId, forts, request.PageSize, queryResult.LastEvaluatedKey);
        }
    }
}
