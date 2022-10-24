namespace TrfrtSbmt.Api.Features.Submissions;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using System.Security.Claims;
using System.Text.Json;
using TrfrtSbmt.Api.DataModels;
using TrfrtSbmt.Api.Utils.DiscordWebhooks;

public class AddSubmission
{
    public record AddSubmissionCommand(string Name, string? State, string? City, string? Country, string? Description, string? Image, string? Website, IEnumerable<string>? Genres, string? Statement, SocialLinksVm Links, ContactInfoVm ContactInfo, string? Id = null) : IRequest<SubmissionViewModel>
    {
        public string? FestivalId { get; internal set; }
        public string? FortId { get; internal set; }
        
    };
    
    public class AddSubmissionCommandHandler : IRequestHandler<AddSubmissionCommand, SubmissionViewModel>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly IAmazonSimpleEmailServiceV2 _emailer;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;

        public AddSubmissionCommandHandler(IAmazonDynamoDB db, IAmazonSimpleEmailServiceV2 emailer, AppSettings settings, ClaimsPrincipal user)
        {
            _db = db;
            _emailer = emailer;
            _settings = settings;
            _user = user;
        }

        public async Task<SubmissionViewModel> Handle(AddSubmissionCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.FestivalId, nameof(request.FestivalId));
            ArgumentNullException.ThrowIfNull(request.FortId, nameof(request.FortId));
            Submission submission;

            // update
            if (request.Id != null)
            {
                var result = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.Id, 1, null);
                var singleOrDefault = result.Items.SingleOrDefault();
                if (singleOrDefault == null) throw new Exception($"Submission not found: {request.Id}");
                submission = new Submission(singleOrDefault);
                submission.Update(
                    request.Name,
                    request.State ?? string.Empty,
                    request.City ?? string.Empty,
                    request.Country ?? string.Empty,
                    request.Description ?? string.Empty,
                    request.Image ?? string.Empty,
                    request.Website ?? string.Empty,
                    request.Genres,
                    request.Statement ?? string.Empty,
                    JsonSerializer.Serialize(request.Links),
                    JsonSerializer.Serialize(request.ContactInfo));
                foreach (var label in submission.Labels)
                {
                    await _db.PutItemAsync(new PutItemRequest(_settings.TableName, new SubmissionLabel(label, submission).ToDictionary()));
                }
                await _db.PutItemAsync(new PutItemRequest(_settings.TableName, submission.ToDictionary()), cancellationToken);
            }
            // add new 
            else
            {
                // ensure festival is open for submissions
                var festivalResponse = await new DynamoDbQueries.EntityIdQuery(_db, _settings).ExecuteAsync(request.FestivalId, 1, null);
                var festival = new Festival(festivalResponse.Items.Single());

                if (DateTime.UtcNow < festival.StartDateTime || DateTime.UtcNow > festival.EndDateTime)
                {
                    throw new Exception("Festival is not open for submissions");
                }

                submission = new Submission(
                request.FestivalId,
                request.FortId,
                _user.Claims.Single(c => c.Type == "username").Value,
                request.Name,
                request.State ?? string.Empty,
                request.City ?? string.Empty,
                request.Country ?? string.Empty,
                request.Description ?? string.Empty,
                request.Image ?? string.Empty,
                request.Website ?? string.Empty,
                request.Genres,
                request.Statement ?? string.Empty,
                JsonSerializer.Serialize(request.Links),
                JsonSerializer.Serialize(request.ContactInfo));
                await _db.PutItemAsync(new PutItemRequest(_settings.TableName, submission.ToDictionary()), cancellationToken);
                if (_settings.EnvironmentName == "Production")
                {
                    try
                    {
                        await _emailer.SendEmailAsync(new SendEmailRequest
                        {
                            FromEmailAddress = _settings.FromEmailAddress,
                            Destination = new Destination
                            {
                                ToAddresses =
                            new List<string> { request.ContactInfo.Email }
                            },
                            Content = new EmailContent
                            {
                                Simple = new Message()
                                {
                                    Subject = new Content
                                    {
                                        Data = "New Submission"
                                    },
                                    Body = new Body
                                    {
                                        Text = new Content
                                        {
                                            Charset = "UTF-8",
                                            Data = @"Thank you for submitting to perform at Treefort Music Fest 2023. Our Artist & Fort Committees are currently reviewing all submissions, please stay tuned for a response. All submitted artists will be notified no later than January 15th, 2023 regarding your status to play at Treefort Music Fest. Submissions can be edited within the Treefort Submissions App, if needed.

Until then, please follow us on our socials and sign up to receive our emails: http://eepurl.com/FO5tT
@treefortfest
#treefort11"
                                        }
                                    }
                                }
                            }
                        });
                    }
                    catch
                    {
                        throw;
                    }                    
                }
            }
            return new SubmissionViewModel(submission);
        }
    }
}
