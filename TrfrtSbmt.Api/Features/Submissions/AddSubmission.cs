﻿namespace TrfrtSbmt.Api.Features.Submissions;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using System.Security.Claims;
using System.Text.Json;
using TrfrtSbmt.Api.DataModels;

public class AddSubmission
{
    public record AddSubmissionCommand(string Name, string State, string City, string Country, string Description, string Image, string Website, IEnumerable<string> Genres, string Statement, SocialLinksVm Links, ContactInfoVm ContactInfo, string? Id = null) : IRequest<SubmissionViewModel>
    {
        public string? FestivalId { get; internal set; }
        public string? FortId { get; internal set; }
        
    };
    
    public class CommandHandler : IRequestHandler<AddSubmissionCommand, SubmissionViewModel>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly IAmazonSimpleEmailServiceV2 _emailer;
        private readonly AppSettings _settings;
        private readonly ClaimsPrincipal _user;
        public CommandHandler(IAmazonDynamoDB db, IAmazonSimpleEmailServiceV2 emailer, AppSettings settings, ClaimsPrincipal user)
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
                    request.State,
                    request.City,
                    request.Country,
                    request.Description,
                    request.Image,
                    request.Website,
                    request.Genres,
                    request.Statement,
                    JsonSerializer.Serialize(request.Links),
                    JsonSerializer.Serialize(request.ContactInfo));
                foreach (var label in submission.Labels)
                {
                    await _db.PutItemAsync(new PutItemRequest(_settings.TableName, new SubmissionLabel(label, submission).ToDictionary()));
                }
            }
            // add new 
            else
            {
                submission = new Submission(
                request.FestivalId,
                request.FortId,
                _user.Claims.Single(c => c.Type == "username").Value,
                request.Name,
                request.State,
                request.City,
                request.Country,
                request.Description,
                request.Image,
                request.Website,
                request.Genres,
                request.Statement,
                JsonSerializer.Serialize(request.Links),
                JsonSerializer.Serialize(request.ContactInfo));
                await _db.PutItemAsync(new PutItemRequest(_settings.TableName, submission.ToDictionary()), cancellationToken);

                if (_settings.EnvironmentName == "Production")
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
                                    //Html = new Content
                                    //{
                                    //    Charset = "UTF-8",
                                    //    Data = htmlBody
                                    //},
                                    Text = new Content
                                    {
                                        Charset = "UTF-8",
                                        Data = @"Your submission to Treefort 11 has been received! We will notify all submissions of their status no later than January 15th, 2023. Submissions can be edited in the Treefort Submissions App."
                                    }
                                }
                            }
                        }
                    });
                }
            }
            return new SubmissionViewModel(submission);
        }
    }
}
