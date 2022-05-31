namespace TrfrtSbmt.Api.Features.Festivals;
using TrfrtSbmt.Api.DataModels;

public record FestivalViewModel(string Id, string Name, string Guidelines, DateTime StartDateTime, DateTime EndDateTime)
{
    public FestivalViewModel(Festival grouping) : this(grouping.EntityId, grouping.Name, grouping.Guidelines, grouping.StartDateTime, grouping.EndDateTime) { }
}
