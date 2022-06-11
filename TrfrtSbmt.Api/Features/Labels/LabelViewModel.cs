using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Labels;

public record LabelViewModel(string Id, string Name)
{
    public LabelViewModel(Fort fort) : this(fort.EntityId, fort.Name) { }
};
