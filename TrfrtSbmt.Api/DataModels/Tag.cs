using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Tag : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Tag)}-";    
    public Tag(string name, List<string> targets) : base(name, name) 
    {
        _attributes[nameof(Targets)] = new AttributeValue { SS = targets };
    }
    public Tag(Dictionary<string, AttributeValue> values) : base(values) { }

    public List<string> Targets => _attributes[nameof(Targets)].SS;
}