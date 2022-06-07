namespace TrfrtSbmt.Api.Features.Festivals;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
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
    /// <param name="Id">Id of the festival. Leave null if creating a new festival. Provide if updating an existing festival.</param>
    public record AddFestivalCommand(bool IsActive, string Name, string Guidelines, DateTime StartDateTime, DateTime EndDateTime, string? Id = null) : IRequest<FestivalViewModel>;
    
    public class CommandHandler : IRequestHandler<AddFestivalCommand, FestivalViewModel>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;

        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        public async Task<FestivalViewModel> Handle(AddFestivalCommand request, CancellationToken cancellationToken)
        {
            Festival festival;
            if (request.Id != null)
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
                if (singleOrDefault == null)
                {
                    throw new Exception($"Festival not found: {request.Id}");
                }
                festival = new Festival(singleOrDefault);
                festival.Update(request.IsActive, request.Name, request.Guidelines, request.StartDateTime, request.EndDateTime);
            }
            else festival = new Festival(request.IsActive, request.Name, request.Guidelines, request.StartDateTime, request.EndDateTime);
            await _db.PutItemAsync(_settings.TableName, festival.ToDictionary(), cancellationToken);
            return new FestivalViewModel(festival);
        }
    }
}