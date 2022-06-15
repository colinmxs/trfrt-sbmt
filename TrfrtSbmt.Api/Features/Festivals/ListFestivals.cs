namespace TrfrtSbmt.Api.Features.Festivals;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text;
using TrfrtSbmt.Api.DataModels;
using TrfrtSbmt.Api.Features;

public record ListFestivalsQuery(bool ActiveOnly = false, bool SubmissionsOpen = false, int PageSize = 20, string? PaginationKey = null) : IRequest<ListFestivalsResult>
{
    internal Dictionary<string, AttributeValue>? ExclusiveStartKey => GetLastEvaluatedKey(PaginationKey);
    private static Dictionary<string, AttributeValue>? GetLastEvaluatedKey(string? paginationKey)
    {
        if (string.IsNullOrEmpty(paginationKey)) return null;
        paginationKey = Encoding.UTF8.GetString(Convert.FromBase64String(paginationKey));
        return
            new Dictionary<string, AttributeValue>
            {
                [nameof(BaseEntity.SearchTerm)] = new AttributeValue { S = nameof(Festival).ToUpperInvariant() },
                [nameof(BaseEntity.PartitionKey)] = new AttributeValue { S = paginationKey.Split('|')[0] },
                [nameof(BaseEntity.SortKey)] = new AttributeValue { S = paginationKey.Split('|')[1] },
                [nameof(BaseEntity.EntityType)] = new AttributeValue { S = nameof(Festival) }
            };
    }
}
public record ListFestivalsResult(IEnumerable<FestivalViewModel> Festivals, int PageSize, string? PaginationKey)
{
    public ListFestivalsResult(IEnumerable<Festival> festivals, int pageSize, Dictionary<string, AttributeValue>? lastEvaluatedKey) :
        this(festivals.Select(f => new FestivalViewModel(f)), pageSize, GetPaginationKey(lastEvaluatedKey))
    { }

    private static string? GetPaginationKey(Dictionary<string, AttributeValue>? lastEvaluatedKey)
    {
        if (lastEvaluatedKey == null) return null;
        if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
        if (!lastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;

        var paginationKey = $"{lastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{lastEvaluatedKey[nameof(BaseEntity.SortKey)].S}";
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(paginationKey));
        return encoded;
    }
}

public class ListFestivalQueryHandler : IRequestHandler<ListFestivalsQuery, ListFestivalsResult>
{
    private readonly IAmazonDynamoDB _db;
    private readonly AppSettings _settings;

    public ListFestivalQueryHandler(IAmazonDynamoDB db, AppSettings settings)
    {
        _db = db;
        _settings = settings;
    }

    public async Task<ListFestivalsResult> Handle(ListFestivalsQuery request, CancellationToken cancellationToken)
    {
        var queryResult = await new DynamoDbQueries.SearchTermQuery(_db, _settings)
            .ExecuteAsync(nameof(Festival), request.PageSize, request.ExclusiveStartKey);

        var festivals = queryResult.Items.Select(i => new Festival(i));
        if (request.ActiveOnly) festivals = festivals.Where(f => f.IsActive).ToList();
        if (request.SubmissionsOpen) festivals = festivals.Where(f => f.StartDateTime <= DateTime.UtcNow && f.EndDateTime >= DateTime.UtcNow).ToList();

        return new ListFestivalsResult(festivals, request.PageSize, queryResult.LastEvaluatedKey);
    }
}