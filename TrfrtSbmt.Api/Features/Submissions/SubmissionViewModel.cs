using System.Text.Json;
using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Submissions;

public record SubmissionViewModel(string FestivalId,
                                      string FortId,
                                      string Id,
                                      string SubmissionDate,
                                      string Name,
                                      string State,
                                      string City,
                                      string Country,
                                      string Description,
                                      string Image,
                                      string Website,
                                      string Statement,
                                      IEnumerable<string> Genres,
                                      SocialLinksVm Links,
                                      ContactInfoVm ContactInfo,
                                      IEnumerable<LabelVm> Labels)
{
    public SubmissionViewModel(Submission submission) : this(
        submission.FestivalId,
        submission.PartitionKey,
        submission.EntityId,
        submission.SubmissionDate,
        submission.Name,
        submission.State,
        submission.City,
        submission.Country,
        submission.Description,
        submission.Image,
        submission.Website,
        submission.Statement,
        submission.Genres,
        JsonSerializer.Deserialize<SocialLinksVm>(submission.Links),
        JsonSerializer.Deserialize<ContactInfoVm>(submission.ContactInfo),
        submission.Labels.Select(l => new LabelVm(l.Id, l.Name))) { }
}

public record LabelVm(string Id, string Name);
public record SocialLinksVm(string? Spotify,
                            string? AppleMusic,
                            string? Bandcamp,
                            string? Soundcloud,
                            string[]? Videos,
                            string? Facebook,
                            string? Twitter,
                            string? Instagram,
                            string? TikTok);
public record ContactInfoVm(string? Name,
                            string? PhoneNumber,
                            string Email,
                            string? Relationship,
                            string? ManagementContact,
                            string? AgentContact,
                            string? PublicityContact,
                            string? LabelContact);
