using Amazon.DynamoDBv2;
using System.Security.Claims;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Submissions;

public class GetMySubmissions
{
    public record GetMySubmissionsQuery : IRequest<List<SubmissionViewModel>>;

    public class GetMySubmissionQueryHandler : IRequestHandler<GetMySubmissionsQuery, List<SubmissionViewModel>>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;
        public GetMySubmissionQueryHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user)
        {
            _db = db;
            _settings = settings;
            _user = user;
        }
        public async Task<List<SubmissionViewModel>> Handle(GetMySubmissionsQuery request, CancellationToken cancellationToken)
        {
            var queryResult = await new DynamoDbQueries.CreatedByQuery(_db, _settings).ExecuteAsync(_user.Claims.Single(c => c.Type == "username").Value);
            return queryResult.Items.Select(i => new SubmissionViewModel(new Submission(i))).ToList();
        }
    }
}
