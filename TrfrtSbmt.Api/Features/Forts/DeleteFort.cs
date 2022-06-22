namespace TrfrtSbmt.Api.Features.Forts;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;

public class DeleteFort
{
    public record DeleteFortCommand(string Id) : IRequest;

    public class CommandHandler : AsyncRequestHandler<DeleteFortCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }

        protected override async Task Handle(DeleteFortCommand request, CancellationToken cancellationToken)
        {
            var fortResult = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.Id, 1, null);
            var singleOrDefault = fortResult.Items.SingleOrDefault();
            if (singleOrDefault != null)
            {
                var fort = new Fort(singleOrDefault);
                List<Dictionary<string, AttributeValue>> items = new();                

                var submissionsResult = await new DynamoDbQueries.Query(_db, _settings).ExecuteAsync(fort.EntityId);

                foreach (var item in submissionsResult.Items)
                {
                    items.Add(item);
                }
                items.Add(fort.ToDictionary());
                await new DynamoDbQueries.DeleteBatch(_db, _settings).ExecuteAsync(items);
            }
        }
    }
}