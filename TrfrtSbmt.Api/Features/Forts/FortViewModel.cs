using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Forts;

public record FortViewModel(string Id, string Name) 
{
    public FortViewModel(Fort fort) : this(fort.EntityId, fort.Name) { }
};
