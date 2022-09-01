namespace TrfrtSbmt.Api.Features.Labels;

using Amazon.DynamoDBv2;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;

public class RemoveLabel
{
    public record RemoveLabelCommand(string LabelId, string SubmissionId) : IRequest;

    public class RemoveLabelCommandHandler : AsyncRequestHandler<RemoveLabelCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public RemoveLabelCommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        
        protected override async Task Handle(RemoveLabelCommand request, CancellationToken cancellationToken)
        {
            // load submission
            var submissionResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.SubmissionId, 1, null);
            var submission = new Submission(submissionResult.Items.Single());
            submission.RemoveLabel(submission.Labels.Single(l => l.Id == request.LabelId));

            // delete submissionlabel
            await _db.DeleteItemAsync(_settings.TableName, new Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue> 
            {
                [nameof(BaseEntity.PartitionKey)] = new Amazon.DynamoDBv2.Model.AttributeValue(request.LabelId),
                [nameof(BaseEntity.SortKey)] = new Amazon.DynamoDBv2.Model.AttributeValue($"{nameof(SubmissionLabel)}-{request.SubmissionId}")
            }, cancellationToken);

            // save submission
            await _db.PutItemAsync(_settings.TableName, submission.ToDictionary(), cancellationToken);
        }
    }
}
