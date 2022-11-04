namespace TrfrtSbmt.Api.Features.Festivals;

using Amazon.DynamoDBv2;
using System.Security.Claims;
using TrfrtSbmt.Domain;
using TrfrtSbmt.Api.Features;

/// <summary>
/// Add a new festival to the database
/// </summary>
/// <param name="Name">The name of the festival.</param>
/// <param name="Guidelines">Submission guidelines for the festival.</param>
/// <param name="StartDateTime">DateTime to start accepting submissions.</param>
/// <param name="EndDateTime">DateTime to stop accepting submissions.</param>
/// <param name="Id">Id of the festival. Leave null if creating a new festival. Provide if updating an existing festival.</param>
public record AddFestivalCommand(bool IsActive, string Name, string Guidelines, DateTime StartDateTime, DateTime EndDateTime, string? Id = null) : IRequest<FestivalViewModel>;

public class AddFestivalCommandHandler : IRequestHandler<AddFestivalCommand, FestivalViewModel>
{
    private readonly IAmazonDynamoDB _db;
    private readonly AppSettings _settings;
    private readonly ClaimsPrincipal _user;
    
    public AddFestivalCommandHandler(IAmazonDynamoDB db, AppSettings settings, ClaimsPrincipal user)
    {
        _db = db;
        _settings = settings;
        _user = user;
    }
    public async Task<FestivalViewModel> Handle(AddFestivalCommand request, CancellationToken cancellationToken)
    {
        Festival festival;
        // update
        if (request.Id != null)
        {
            var result = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.Id, 1, null);
            var singleOrDefault = result.Items.SingleOrDefault();
            if (singleOrDefault == null) throw new Exception($"Festival not found: {request.Id}");
            festival = new Festival(singleOrDefault);
            festival.Update(request.IsActive, request.Name, request.Guidelines, request.StartDateTime, request.EndDateTime);
        }
        // add new
        else festival = new Festival(request.IsActive, request.Name, _user.Claims.Single(c => c.Type == "username").Value, request.Guidelines, request.StartDateTime, request.EndDateTime);
        await _db.PutItemAsync(_settings.TableName, festival.ToDictionary(), cancellationToken);
        return new FestivalViewModel(festival);
    }
}