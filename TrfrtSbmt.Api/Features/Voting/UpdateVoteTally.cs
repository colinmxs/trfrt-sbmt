namespace TrfrtSbmt.Api.Features.Voting;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TrfrtSbmt.Api.DataModels;

public class UpdateVoteTally
{
    public record UpdateVoteTallyCommand() : IRequest;
    public class UpdateVoteTallyCommandHandler : AsyncRequestHandler<UpdateVoteTallyCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public UpdateVoteTallyCommandHandler(IAmazonDynamoDB dynamoDbClient, AppSettings settings)
        {
            _db = dynamoDbClient;
            _settings = settings;
        }

        protected override async Task Handle(UpdateVoteTallyCommand request, CancellationToken cancellationToken)
        {
            // scan table get all data
            var scanResult = await _db.ScanAsync(new ScanRequest(_settings.TableName), cancellationToken);
            var items = scanResult.Items;
            while (scanResult.LastEvaluatedKey != null)
            {
                scanResult = await _db.ScanAsync(new ScanRequest(_settings.TableName)
                {
                    ExclusiveStartKey = scanResult.LastEvaluatedKey
                }, cancellationToken);
                items.AddRange(scanResult.Items);
            }

            // get only votes
            var votes = items.Where(i => i["SortKey"].S.StartsWith(nameof(Vote) + "-")).Select(i => new Vote(i)).ToList();
            var voteGrouping = votes.GroupBy(v => v.PartitionKey);
            foreach (var group in voteGrouping)
            {
                var submission = items.SingleOrDefault(i => i["PartitionKey"].S == group.Key && i["SortKey"].S.StartsWith(nameof(Submission) + "-"));
                if (submission == null) continue;
                var submissionModel = new Submission(submission);
                var submissionVotes = group.ToList();
                var rank = submissionVotes.Sum(v => v.Value) / group.Count();
                var submissionRank = new SubmissionRank(submissionModel, rank, group.Count());
                await _db.PutItemAsync(new PutItemRequest(_settings.TableName, submissionRank.ToDictionary()), cancellationToken);
            }
        }
    }
}
