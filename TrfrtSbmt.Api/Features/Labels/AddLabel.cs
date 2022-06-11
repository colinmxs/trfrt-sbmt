using Amazon.DynamoDBv2;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Labels;

public class AddLabel
{
    public record AddLabelCommand(string Name) : IRequest<LabelViewModel>
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
            var fort = new Fort(request.FestivalId, request.Name);
            await _db.PutItemAsync(_settings.TableName, fort.ToDictionary(), cancellationToken);
            return new LabelViewModel(fort);
        }
    }
}
