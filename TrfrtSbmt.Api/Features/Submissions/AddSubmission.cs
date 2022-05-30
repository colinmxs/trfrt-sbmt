namespace TrfrtSbmt.Api.Features.Submissions;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text.Json;
using TrfrtSbmt.Api.DataModels;

public class AddSubmission
{
    public record AddSubmissionCommand(string Name, string State, string City, string Country, string Description, string Image, string Website, string Genre, SocialLinksVm Links, ContactInfoVm ContactInfo) : IRequest 
    {
        public string? FestivalId { get; internal set; }
        public string? FortId { get; internal set; }
    };
    public record SocialLinksVm(string Spotify, string AppleMusic, string Bandcamp, string Soundcloud, string[] Videos, string Facebook, string Twitter, string Instagram, string TikTok);    
    public record ContactInfoVm(string Name, string PhoneNumber, string Email, string Relationship, string ManagementContact, string AgentContact, string PublicityContact, string LabelContact);
    public class CommandHandler : AsyncRequestHandler<AddSubmissionCommand>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly AppSettings _settings;
        public CommandHandler(IAmazonDynamoDB db, AppSettings settings)
        {
            _db = db;
            _settings = settings;
        }
        protected override async Task Handle(AddSubmissionCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.FestivalId, nameof(request.FestivalId));
            ArgumentNullException.ThrowIfNull(request.FortId, nameof(request.FortId));
            await _db.PutItemAsync(new PutItemRequest(_settings.TableName, 
                new Submission(
                    request.FestivalId,
                    request.FortId, 
                    request.Name, 
                    request.State, 
                    request.City, 
                    request.Country, 
                    request.Description, 
                    request.Image, 
                    request.Website, 
                    request.Genre, 
                    JsonSerializer.Serialize(request.Links), 
                    JsonSerializer.Serialize(request.ContactInfo)).ToDictionary()));
        }
    }
}
