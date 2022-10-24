using Amazon.DynamoDBv2;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Voting
{
    public class GetVoteTally
    {
        public record GetVoteTallyQuery(string FestivalId, string FortId) : IRequest<List<VoteTallyResult>>;
        public record VoteTallyResult(string Id, string Name, string State, string City, string Country, string Image, string FortId, string AverageScore, string NumberOfVotes);
        public class GetVoteTallyQueryHandler : IRequestHandler<GetVoteTallyQuery, List<VoteTallyResult>>
        {
            private readonly IAmazonDynamoDB _db;
            private readonly AppSettings _settings;

            public GetVoteTallyQueryHandler(IAmazonDynamoDB dynamoDbClient, AppSettings settings)
            {
                _db = dynamoDbClient;
                _settings = settings;
            }
            
            public async Task<List<VoteTallyResult>> Handle(GetVoteTallyQuery request, CancellationToken cancellationToken)
            {
                var queryResult = await new DynamoDbQueries.VoteTallyQuery(_db, _settings).ExecuteAsync(request.FortId);
                var ranks = queryResult.Items.Select(i => new SubmissionRank(i));
                return ranks.Select(r => new VoteTallyResult(r.SubmissionEntityId, r.Name, r.State, r.City, r.Country, r.Image, r.FortId, r.Rank, r.NumberOfVotes)).ToList();
            }
        }
    }
}
