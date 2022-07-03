namespace TrfrtSbmt.Api.Features.Labels;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;

public class DeleteLabel
{
    public record DeleteLabelCommand(string Id) : IRequest;

    public class CommandHandler : AsyncRequestHandler<DeleteLabelCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        protected override async Task Handle(DeleteLabelCommand request, CancellationToken cancellationToken)
        {
            var labelResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.Id, 1, null);
            var singleOrDefault = labelResult.Items.SingleOrDefault();
            if (singleOrDefault != null) 
            {
                var label = new Label(singleOrDefault);
                List<Dictionary<string, AttributeValue>> items = new();
                var submissionLabelsResult = await new DynamoDbQueries.Query(_db, _settings).ExecuteAsync(label.EntityId);
                foreach (var item in submissionLabelsResult.Items.Select(i => new SubmissionLabel(i)))
                {
                    items.Add(item.ToDictionary());

                    var submissionId = submissionLabelsResult.Items.Single()[nameof(SubmissionLabel.SubmissionEntityId)].S;
                    var submissionResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(submissionId, 1, null);
                    var submission = new Submission(submissionResult.Items.Single());
                    submission.RemoveLabel(submission.Labels.SingleOrDefault(l => l.Id == item.PartitionKey));
                    await _db.PutItemAsync(_settings.TableName, submission.ToDictionary());
                }
                
                items.Add(label.ToDictionary());
                await new DynamoDbQueries.DeleteBatch(_db, _settings).ExecuteAsync(items);                
            }
        }
    }
}