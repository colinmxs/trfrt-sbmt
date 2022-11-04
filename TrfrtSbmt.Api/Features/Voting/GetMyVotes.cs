using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using System.Security.Claims;
using TrfrtSbmt.Domain;

namespace TrfrtSbmt.Api.Features.Voting;

public class GetMyVotes
{
    public record GetMyVotesQuery : IRequest<List<VoteViewModel>>;

    public record VoteViewModel(string SubmissionId, int Value)
    {
        public VoteViewModel(Vote vote) : this(vote.PartitionKey, vote.Value) { }
    }

    public class GetMyVotesQueryHandler : IRequestHandler<GetMyVotesQuery, List<VoteViewModel>>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;

        public GetMyVotesQueryHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user)
        {
            _db = db;
            _settings = settings;
            _user = user;
        }

        public async Task<List<VoteViewModel>> Handle(GetMyVotesQuery request, CancellationToken cancellationToken)
        {
            var result = new List<VoteViewModel>();
            var votesResponse = await new DynamoDbQueries.CreatedByQuery(_db, _settings).ExecuteAsync(_user.Claims.Single(c => c.Type == "username").Value, nameof(Vote));
            foreach (var item in votesResponse.Items)
            {
                var vote = new Vote(item);
                result.Add(new VoteViewModel(vote));
            }
            return result;
        }        
    }
}
