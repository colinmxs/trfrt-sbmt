using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Groupings;

public class DefineSubmissionGrouping
{ 
    public record Command(string GroupingName, List<string> SubGroupings, string Guidelines, DateTime StartDateTime, DateTime EndDateTime) : IRequest;
    
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
            var grouping = new Grouping(request.GroupingName, request.SubGroupings, request.Guidelines, request.StartDateTime, request.EndDateTime);
            await _db.PutItemAsync(_settings.TableName, grouping.ToDictionary());
        }
    }
}