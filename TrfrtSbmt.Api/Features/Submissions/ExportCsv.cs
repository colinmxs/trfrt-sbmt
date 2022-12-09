namespace TrfrtSbmt.Api.Features.Submissions;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Security.Claims;
using System.Text;
using TrfrtSbmt.Domain;
public class ExportCsv
{
    public record ExportCsvQuery(string FestivalId, string? FortId) : IRequest<ExportCsvResult>;

    public record ExportCsvResult(string FestivalId, string? FortId, IEnumerable<SubmissionViewModel> Submissions)
    {
        public ExportCsvResult(string festivalId, string? fortId, IEnumerable<Submission> submissions) :
        this(festivalId, fortId, submissions.Select(s => new SubmissionViewModel(s)))
        { }        
    }
    
    public class QueryHandler : IRequestHandler<ExportCsvQuery, ExportCsvResult>
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
        public async Task<ExportCsvResult> Handle(ExportCsvQuery request, CancellationToken cancellationToken)
        {
            List<Submission> submissions = new List<Submission>();
            QueryResponse? queryResult = null;

            string[] fortIds;
            if (request.FortId != null)
            {
                fortIds = new string[] { request.FortId };
            }
            else
            {
                var fortsResult = await new DynamoDbQueries.BeginsWithQuery(_db, _settings).ExecuteAsync(request.FestivalId, $"{nameof(Fort)}-", 10000, null);
                fortIds = fortsResult.Items.Select(i => i[nameof(Fort.EntityId)].S).ToArray();
            }            

            if ((_user.IsAdmin() || _user.IsVoter()))
            {
                ArgumentNullException.ThrowIfNull(request.FortId);
                queryResult = await SubmissionDateIndexQuery(_db, _settings, request.FortId);
                submissions = queryResult.Items.Select(i => new Submission(i)).ToList();
            }

            return new ExportCsvResult(request.FestivalId, request.FortId, submissions);
        }

        private static async Task<QueryResponse> SubmissionDateIndexQuery(IAmazonDynamoDB db, AppSettings settings, string id)
        {
            var pkSymbol = ":partitionKey";
                
            var resultSet = await db.QueryAsync(new QueryRequest(settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [pkSymbol] = new AttributeValue { S = id }
                },
                IndexName = "SubmissionDateIndex"
            });

            // handle pagination
            while (resultSet.LastEvaluatedKey != null)
            {
                var nextResultSet = await db.QueryAsync(new QueryRequest(settings.TableName)
                {
                    KeyConditionExpression = $"{nameof(BaseEntity.PartitionKey)} = {pkSymbol}",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [pkSymbol] = new AttributeValue { S = id }
                    },
                    IndexName = "SubmissionDateIndex",
                    ExclusiveStartKey = resultSet.LastEvaluatedKey
                });
                resultSet.Items.AddRange(nextResultSet.Items);
                resultSet.LastEvaluatedKey = nextResultSet.LastEvaluatedKey;
            }

            return resultSet;
        }
    }
}