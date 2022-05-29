using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text.Json;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Submissions;

public class Submit
{
    public record Command(string SubGroupingId, string Name, string State, string City, string Country, string Description, string Image, string Website, string Genre, SocialLinksVm Links, ContactInfoVm ContactInfo) : IRequest;
    public record SocialLinksVm(string Spotify, string AppleMusic, string Bandcamp, string Soundcloud, string[] Videos, string Facebook, string Twitter, string Instagram, string TikTok);    
    public record ContactInfoVm(string Name, string PhoneNumber, string Email, string Relationship, string ManagementContact, string AgentContact, string PublicityContact, string LabelContact);
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
            //await _db.PutItemAsync(new PutItemRequest(_settings.TableName, new Submission().ToDictionary()));
        }
    }
}
