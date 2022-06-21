using TrfrtSbmt.Api.DataModels;

namespace TrfrtSbmt.Api.Features.Labels;

public record LabelViewModel(string Id, string Name)
{
    public LabelViewModel(Label label) : this(label.EntityId, label.Name) { }
};
