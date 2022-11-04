namespace TrfrtSbmt.Api.Features.Festivals;

using TrfrtSbmt.Domain;

public record FestivalViewModel(string Id, bool IsActive, string Name, string Guidelines, DateTime StartDateTime, DateTime EndDateTime)
{
    public FestivalViewModel(Festival festival) : this(festival.EntityId, festival.IsActive, festival.Name, festival.Guidelines, festival.StartDateTime, festival.EndDateTime) { }
}