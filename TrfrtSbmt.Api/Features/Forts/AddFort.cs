using Amazon.DynamoDBv2;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Forts;

public class AddFort
{
    public record AddFortCommand(string Name) : IRequest 
    {
        public string? FestivalId { get; internal set; }
    }

    public class CommandHandler : AsyncRequestHandler<AddFortCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        
        protected override async Task Handle(AddFortCommand request, CancellationToken cancellationToken)
        {
            var fort = new Fort(request.FestivalId, request.Name);
            await _db.PutItemAsync(_settings.TableName, fort.ToDictionary(), cancellationToken);
        }
    }
}
