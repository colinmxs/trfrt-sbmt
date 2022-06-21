namespace TrfrtSbmt.Api.Features.Labels;

using Amazon.DynamoDBv2;
using TrfrtSbmt.Api.DataModels;

public class AddLabel
{
    public record AddLabelCommand(string Name, string FortId, string SubmissionId) : IRequest<LabelViewModel>
    {
        public string? FestivalId { get; internal set; }
    }

    public class CommandHandler : IRequestHandler<AddLabelCommand, LabelViewModel>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        public async Task<LabelViewModel> Handle(AddLabelCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.FestivalId);
            Label label;
            
            var festivalLabelResult = await new DynamoDbQueries.Query(_db, _settings).ExecuteAsync(request.FestivalId, $"{nameof(Label)}-{request.Name.ToUpperInvariant()}");
            var singleOrDefaultFestivalLabel = festivalLabelResult.Items.SingleOrDefault();
            if (singleOrDefaultFestivalLabel == null)
            {
                label = new Label(request.FestivalId, request.Name);
                await _db.PutItemAsync(_settings.TableName, label.ToDictionary(), cancellationToken);
            }
            else label = new Label(singleOrDefaultFestivalLabel);

            var submissionResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.SubmissionId, 1, null);
            var singleOrDefault = submissionResult.Items.SingleOrDefault();
            if(singleOrDefault != null)
            {
                var labeledSubmission = new Submission(label.EntityId, singleOrDefault);
                await _db.PutItemAsync(_settings.TableName, labeledSubmission.ToDictionary(), cancellationToken);
            }

            return new LabelViewModel(label);
        }
    }
}
