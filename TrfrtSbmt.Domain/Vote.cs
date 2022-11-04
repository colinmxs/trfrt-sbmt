namespace TrfrtSbmt.Domain;

using Amazon.DynamoDBv2.Model;

public sealed class Vote : BaseEntity
{
    public Vote(Dictionary<string, AttributeValue> values) : base(values) { }

    public Vote(string submissionId, int value, string voter) : base(submissionId, voter, $"{nameof(Vote)}-{voter}", $"{nameof(Vote)}-{voter}", voter)
    {
        _attributes[nameof(Value)] = new AttributeValue { S = value.ToString() };
    }

    public override string SortKeyPrefix => $"{nameof(Vote)}-";

    public int Value => int.Parse(_attributes[nameof(Value)].S);
}
