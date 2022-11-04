using Amazon.DynamoDBv2;
using System.Security.Claims;
using TrfrtSbmt.Domain;

namespace TrfrtSbmt.Api.Features.Forts;

public class AddFort
{
    public record AddFortCommand(string Name, string Description) : IRequest<FortViewModel>
    {
        public string? FestivalId { get; internal set; }
    }

    public class CommandHandler : IRequestHandler<AddFortCommand, FortViewModel>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user)
        {
            _db = db;
            _settings = settings;
            _user = user;
        }

        public async Task<FortViewModel> Handle(AddFortCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.FestivalId);
            var fort = new Fort(request.FestivalId, request.Name, request.Description, _user.Claims.Single(c => c.Type == "username").Value);
            await _db.PutItemAsync(_settings.TableName, fort.ToDictionary(), cancellationToken);
            return new FortViewModel(fort);
        }
    }
}
