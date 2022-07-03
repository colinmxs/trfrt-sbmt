namespace TrfrtSbmt.Api.Features.Festivals;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;
using TrfrtSbmt.Api.Features;

public record DeleteFestivalCommand(string Id) : IRequest;

public class DeleteFestivalCommandHandler : AsyncRequestHandler<DeleteFestivalCommand>
{
    private readonly IAmazonDynamoDB _db;
    private readonly AppSettings _settings;

    public DeleteFestivalCommandHandler(IAmazonDynamoDB db, AppSettings settings)
    {
        _db = db;
        _settings = settings;
    }

    protected override async Task Handle(DeleteFestivalCommand request, CancellationToken cancellationToken)
    {
        var result = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.Id, 1, null);
        var singleOrDefault = result.Items.SingleOrDefault();
        if (singleOrDefault != null)
        {
            var festival = new Festival(singleOrDefault);
            List<Fort> forts = new();
            List<Label> labels = new();
            List<Dictionary<string, AttributeValue>> items = new();
            
            List<Dictionary<string, AttributeValue>> dictionaries = new();
            QueryResponse? fortsResult = null;
            do
            {
                fortsResult = await new DynamoDbQueries.BeginsWithQuery(_db, _settings).ExecuteAsync(festival.EntityId, nameof(Fort), 1000, fortsResult?.LastEvaluatedKey);
                dictionaries.AddRange(fortsResult.Items);
            } while (fortsResult.LastEvaluatedKey.Count() != 0);

            forts.AddRange(dictionaries.Select(i => new Fort(i)));

            foreach (var fort in forts)
            {
                var submissionsResult = await new DynamoDbQueries.Query(_db, _settings).ExecuteAsync(fort.EntityId);

                foreach (var item in submissionsResult.Items)
                {
                    items.Add(item);
                }
                items.Add(fort.ToDictionary());
            }
            items.Add(festival.ToDictionary());

            dictionaries = new();
            QueryResponse? labelsResult = null;
            do
            {
                labelsResult = await new DynamoDbQueries.BeginsWithQuery(_db, _settings).ExecuteAsync(festival.EntityId, nameof(Label), 1000, fortsResult?.LastEvaluatedKey);
                dictionaries.AddRange(labelsResult.Items);
            } while (labelsResult.LastEvaluatedKey.Count() != 0);
            labels.AddRange(dictionaries.Select(i => new Label(i)));

            foreach (var label in labels)
            {
                var labelSubmissionsResult = await new DynamoDbQueries.Query(_db, _settings).ExecuteAsync(label.EntityId);

                foreach (var item in labelSubmissionsResult.Items)
                {
                    items.Add(item);
                }
                items.Add(label.ToDictionary());
            }

            await new DynamoDbQueries.DeleteBatch(_db, _settings).ExecuteAsync(items.Distinct().ToList());
        }
    }

}
