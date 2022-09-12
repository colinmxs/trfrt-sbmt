namespace TrfrtSbmt.Api.Features.Voting;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Security.Claims;
using TrfrtSbmt.Api.DataModels;

public class VoteOnSubmission
{
    public record VoteOnSubmissionCommand(string SubmissionId, int Value) : IRequest;

    public class VoteOnSubmissionCommandHandler : AsyncRequestHandler<VoteOnSubmissionCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;

        public VoteOnSubmissionCommandHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user)
        {
            _db = db;
            _settings = settings;
            _user = user;
        }

        protected override async Task Handle(VoteOnSubmissionCommand request, CancellationToken cancellationToken)
        {
            var vote = new Vote(request.SubmissionId, request.Value, _user.Claims.Single(c => c.Type == "username").Value);
            await _db.PutItemAsync(new PutItemRequest(_settings.TableName, vote.ToDictionary()), cancellationToken);
        }
    }

}
