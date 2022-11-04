namespace TrfrtSbmt.Domain;

using Amazon.DynamoDBv2.Model;

public sealed class Label : BaseEntity
{
    public override string SortKeyPrefix => $"{nameof(Label)}-";
    public Label(string festivalId, string name, string createdBy) : base(festivalId, name.ToUpperInvariant(), name, createdBy) { }
    public Label(Dictionary<string, AttributeValue> values) : base(values) { }
}