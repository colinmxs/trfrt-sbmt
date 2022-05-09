using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Text.Json;

namespace TrfrtSbmt.Api.Features;

public class Submit
{
    public record Command(string Fort, string Name, string State, string City, string Country, string Description, string Image, string Website, string Genre, SocialLinksVm Links, ContactInfoVm ContactInfo) : IRequest;
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
            await _db.PutItemAsync(new PutItemRequest(_settings.TableName, new Dictionary<string, AttributeValue>
            {
                ["PartitionKey"] = new AttributeValue { S = request.Name },
                ["SortKey"] = new AttributeValue { S = request.Fort },
                ["SubmissionGrouping"] = new AttributeValue { S = _settings.SubmissionGrouping },
                ["State"] = new AttributeValue { S = request.State },
                ["City"] = new AttributeValue { S = request.City },
                ["Country"] = new AttributeValue { S = request.Country },
                ["Description"] = new AttributeValue { S = request.Description },
                ["Image"] = new AttributeValue { S = request.Image },
                ["Website"] = new AttributeValue { S = request.Website },
                ["Genre"] = new AttributeValue { S = request.Genre },
                ["Links"] = new AttributeValue { S = JsonSerializer.Serialize(request.Links)},
                ["ContactInfo"] = new AttributeValue { S = JsonSerializer.Serialize(request.ContactInfo) }
            }));
        }
    }
}
