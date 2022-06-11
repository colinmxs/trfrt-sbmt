using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Submissions;
public class GetSubmission
{
    public record GetSubmissionQuery(string FestivalId, string FortId, string SubmissionId) : IRequest<SubmissionViewModel>;

    public class QueryHandler : IRequestHandler<GetSubmissionQuery, SubmissionViewModel>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public QueryHandler(IAmazonDynamoDB dynamoDbClient, AppSettings settings)
        {
            _db = dynamoDbClient;
            _settings = settings;
        }

        public async Task<SubmissionViewModel> Handle(GetSubmissionQuery request, CancellationToken cancellationToken)
        {
            var queryResult = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.EntityId)} = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":id", new AttributeValue(request.SubmissionId)}
                },
                IndexName = BaseEntity.Gsi2
            });
            var singleOrDefault = queryResult.Items.SingleOrDefault();
            if (singleOrDefault == null) throw new Exception("Submission not found");

            var submission = new Submission(singleOrDefault);
            return new SubmissionViewModel(submission);
        }
    }
}