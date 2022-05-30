namespace TrfrtSbmt.Api.Features.Festivals;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading;
using System.Threading.Tasks;
using TrfrtSbmt.Api.DataModels;

public class DeleteFestival
{
    public record DeleteFestivalCommand(string Id) : IRequest;

    public class CommandHandler : AsyncRequestHandler<DeleteFestivalCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        
        protected override async Task Handle(DeleteFestivalCommand request, CancellationToken cancellationToken)
        {
            var result = await _db.QueryAsync(new QueryRequest(_settings.TableName)
            {
                KeyConditionExpression = $"{nameof(BaseEntity.EntityId)} = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":id", new AttributeValue(request.Id)}
                },
                IndexName = BaseEntity.Gsi2
            });
            var singleOrDefault = result.Items.SingleOrDefault();
            if (singleOrDefault == null) throw new Exception("Festival not found");
            var festival = new Festival(singleOrDefault);
            await festival.DeleteAsync(_db, _settings.TableName);            
        }
    }
}
