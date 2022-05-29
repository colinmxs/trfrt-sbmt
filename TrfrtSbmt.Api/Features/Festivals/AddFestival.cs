namespace TrfrtSbmt.Api.Features.Festivals;

using Amazon.DynamoDBv2;
using TrfrtSbmt.Api.DataModels;

public class AddFestival
{
    /// <summary>
    /// Add a new festival to the database
    /// </summary>
    /// <param name="Name">The name of the festival.</param>
    /// <param name="Guidelines">Submission guidelines for the festival.</param>
    /// <param name="StartDateTime">DateTime to start accepting submissions.</param>
    /// <param name="EndDateTime">DateTime to stop accepting submissions.</param>
    public record Command(string Name, string Guidelines, DateTime StartDateTime, DateTime EndDateTime) : IRequest;
    
    public class CommandHandler : AsyncRequestHandler<Command>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var grouping = new Festival(request.Name, request.Guidelines, request.StartDateTime, request.EndDateTime);
            await _db.PutItemAsync(_settings.TableName, grouping.ToDictionary(), cancellationToken);
        }
    }
}