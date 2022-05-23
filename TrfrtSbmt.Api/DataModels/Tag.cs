using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Tag : BaseEntity
{
    private const string PartitionKeyValuePrefix = "Tag-";
    public Tag(string name, List<string> targets) : base($"{PartitionKeyValuePrefix}{name}", name, nameof(Tag)) 
    {
        _attributes[nameof(Name)] = new AttributeValue { S = name };
        _attributes[nameof(Targets)] = new AttributeValue { SS = targets };
    }
    public Tag(Dictionary<string, AttributeValue> values) : base(values) { }

    public string Name => _attributes[nameof(Name)].S;
    public List<string> Targets => _attributes[nameof(Targets)].SS;
}