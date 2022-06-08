using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text.Json;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Submissions;

public class ListSubmissions
{
    public record ListSubmissionsQuery(string FestivalId, string FortId, int PageSize = 20, string? PaginationKey = null) : IRequest<ListSubmissionsResult>;

    public record ListSubmissionsResult(string FestivalId, string FortId, List<SubmissionViewModel> Submissions, int PageSize = 20, string? PaginationKey = null);
    public record SubmissionViewModel(string FestivalId,
                                      string FortId,
                                      string SubmissionDate,
                                      string Name,
                                      string State,
                                      string City,
                                      string Country,
                                      string Description,
                                      string Image,
                                      string Website,
                                      string Genre,
                                      object Links,
                                      object ContactInfo);
    
    public class QueryHandler : IRequestHandler<ListSubmissionsQuery, ListSubmissionsResult>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        public async Task<ListSubmissionsResult> Handle(ListSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var pkSymbol = ":partitionKey";
            var queryResult = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = request.FortId }
                },
                Limit = request.PageSize,
                ExclusiveStartKey = GetLastEvaluatedKey(request),
                IndexName = "SubmissionDateIndex",
                ScanIndexForward = false
            });
            if (queryResult.Items == null) return new ListSubmissionsResult(request.FestivalId, request.FortId, new List<SubmissionViewModel>(), request.PageSize, null);
            var submissions = queryResult.Items.Select(i => new Submission(i));

            string? paginationKey = GetPaginationKey(queryResult);
            return new ListSubmissionsResult(request.FestivalId, request.FortId, submissions.Select(f => new SubmissionViewModel(request.FestivalId, request.FortId, f.SubmissionDate, f.Name, f.State, f.City, f.Country, f.Description, f.Image, f.Website, f.Genre, JsonSerializer.Deserialize<object>(f.Links), JsonSerializer.Deserialize<object>(f.ContactInfo))).ToList(), request.PageSize, paginationKey);
        }

        private static string? GetPaginationKey(QueryResponse queryResult)
        {
            if (queryResult.LastEvaluatedKey == null) return null;
            if (!queryResult.LastEvaluatedKey.ContainsKey(nameof(BaseEntity.PartitionKey))) return null;
            if (!queryResult.LastEvaluatedKey.ContainsKey(nameof(BaseEntity.SortKey))) return null;

            return $"{queryResult.LastEvaluatedKey[nameof(BaseEntity.PartitionKey)].S}|{queryResult.LastEvaluatedKey[nameof(BaseEntity.SortKey)].S}";
        }

        private Dictionary<string, AttributeValue> GetLastEvaluatedKey(ListSubmissionsQuery request)
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