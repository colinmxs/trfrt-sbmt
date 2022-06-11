using System.Text.Json;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Submissions;

public record SubmissionViewModel(string FestivalId,
                                      string FortId,
                                      string SubmissionDate,
                                      string Name,
                                      string State,
                                      string City,
                                      string Country,
                                      string Description,
                                      string Image,
                                      string Website,
                                      string Genre,
                                      SocialLinksVm Links,
                                      ContactInfoVm ContactInfo)
{
    public SubmissionViewModel(Submission submission) : this(submission.FestivalId, submission.PartitionKey, submission.SubmissionDate, submission.Name, submission.State, submission.City, submission.Country, submission.Description, submission.Image, submission.Website, submission.Genre, JsonSerializer.Deserialize<SocialLinksVm>(submission.Links), JsonSerializer.Deserialize<ContactInfoVm>(submission.ContactInfo)) { }
}

public record SocialLinksVm(string? Spotify, string? AppleMusic, string? Bandcamp, string? Soundcloud, string[]? Videos, string? Facebook, string? Twitter, string? Instagram, string? TikTok);
public record ContactInfoVm(string? Name, string? PhoneNumber, string? Email, string? Relationship, string? ManagementContact, string? AgentContact, string? PublicityContact, string? LabelContact);
