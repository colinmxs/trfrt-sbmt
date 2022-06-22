using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Label : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Label)}-";
    public Label(string festivalId, string name) : base(festivalId, name.ToUpperInvariant(), name) { }
    public Label(Dictionary<string, AttributeValue> values) : base(values) { }
}