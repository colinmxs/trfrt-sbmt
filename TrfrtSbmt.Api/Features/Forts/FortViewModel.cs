using TrfrtSbmt.Domain;

namespace TrfrtSbmt.Api.Features.Forts;

public record FortViewModel(string Id, string Name, string Description) 
{
    public FortViewModel(Fort fort) : this(fort.EntityId, fort.Name, fort.Description) { }
};
