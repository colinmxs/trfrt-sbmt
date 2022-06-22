namespace TrfrtSbmt.Api.Features.Submissions;

using Amazon.DynamoDBv2;
using TrfrtSbmt.Api.DataModels;

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
            var queryResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.SubmissionId, 1, null);              
            var singleOrDefault = queryResult.Items.SingleOrDefault();
            if (singleOrDefault == null) throw new Exception("Submission not found");
            var submission = new Submission(singleOrDefault);
            return new SubmissionViewModel(submission);
        }
    }
}