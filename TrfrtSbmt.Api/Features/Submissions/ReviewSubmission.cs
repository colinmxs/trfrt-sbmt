using Amazon.DynamoDBv2;
using Amazon.SimpleEmailV2;
using System.Security.Claims;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Submissions
{
    public class ReviewSubmission
    {
        public enum ReviewAction
        {
            Approve,
            Reject,
            Rescind
        }

        public record ReviewSubmissionCommand(ReviewAction Action) : IRequest 
        {
            public string? FestivalId { get; set; }
            public string? FortId { get; set; }
            public string? SubmissionId { get; set; }
        }

        public class ReviewSubmissionCommandHandler : AsyncRequestHandler<ReviewSubmissionCommand>
        {
            private readonly IAmazonDynamoDB _db;
            private readonly IAmazonSimpleEmailServiceV2 _emailer;
            private readonly AppSettings _settings;
            private readonly ClaimsPrincipal _user;
            public ReviewSubmissionCommandHandler(IAmazonDynamoDB db, IAmazonSimpleEmailServiceV2 emailer, AppSettings settings, ClaimsPrincipal user)
            {
                _db = db;
                _emailer = emailer;
                _settings = settings;
                _user = user;
            }

            protected override async Task Handle(ReviewSubmissionCommand request, CancellationToken cancellationToken)
            {
                ArgumentNullException.ThrowIfNull(request.SubmissionId);
                
                var submissionResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.SubmissionId, 1, null);
                var submission = new Submission(submissionResult.Items.Single());

                switch (request.Action)
                {
                    case ReviewAction.Approve:
                        submission.Approve(_user.Claims.Single(c => c.Type == "username").Value);
                        break;
                    case ReviewAction.Reject:
                        submission.Reject(_user.Claims.Single(c => c.Type == "username").Value);
                        break;
                    case ReviewAction.Rescind:
                        submission.RescindReview();
                        break;
                    default:
                        break;
                }

                await _db.PutItemAsync(_settings.TableName, submission.ToDictionary());
            }
        }
    }
}
