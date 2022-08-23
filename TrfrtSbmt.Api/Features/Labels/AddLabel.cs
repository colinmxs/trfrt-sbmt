namespace TrfrtSbmt.Api.Features.Labels;

using Amazon.DynamoDBv2;
using System.Security.Claims;
using TrfrtSbmt.Api.DataModels;

public class AddLabel
{
    public record AddLabelCommand(string Name, string[]? SubmissionIds) : IRequest<LabelViewModel>
    {
        public string? FestivalId { get; internal set; }
    }

    public class AddLabelCommandHandler : IRequestHandler<AddLabelCommand, LabelViewModel>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;

        public AddLabelCommandHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user)
        {
            _db = db;
            _settings = settings;
            _user = user;
        }

        public async Task<LabelViewModel> Handle(AddLabelCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.FestivalId);
            Label label;
            
            var festivalLabelResult = await new DynamoDbQueries.SearchTermQuery(_db, _settings).ExecuteAsync($"{request.Name.ToUpperInvariant()}", nameof(Label), 1, null);
            var singleOrDefaultFestivalLabel = festivalLabelResult.Items.SingleOrDefault();
            if (singleOrDefaultFestivalLabel == null)
            {
                label = new Label(request.FestivalId, request.Name, _user.Claims.Single(c => c.Type == "username").Value);
                await _db.PutItemAsync(_settings.TableName, label.ToDictionary(), cancellationToken);
            }
            else label = new Label(singleOrDefaultFestivalLabel);

            if(request.SubmissionIds != null)
            {
                foreach (var submissionId in request.SubmissionIds)
                {
                    var submissionResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(submissionId, 1, null);
                    var singleOrDefault = submissionResult.Items.SingleOrDefault();
                    if(singleOrDefault != null)
                    {
                        var submission = new Submission(singleOrDefault);
                        var submissionLabel = new SubmissionLabel(label, submission, _user.Claims.Single(c => c.Type == "username").Value);
                        submission.AddLabel(new Submission.Label(submissionLabel));
                        await _db.PutItemAsync(_settings.TableName, submissionLabel.ToDictionary(), cancellationToken);
                        await _db.PutItemAsync(_settings.TableName, submission.ToDictionary(), cancellationToken);
                    }
                }
            }

            return new LabelViewModel(label);
        }
    }
}
