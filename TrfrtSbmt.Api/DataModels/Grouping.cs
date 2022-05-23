using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Grouping : BaseEntity
{
    private const string PartitionKeyValuePrefix = "Grouping-";
    public Grouping(string name, List<SubGrouping> subGroupings, string guidelines, DateTime startDateTime, DateTime endDateTime) : base($"{PartitionKeyValuePrefix}{name}", nameof(Grouping), nameof(Grouping))
    {
        _attributes[nameof(Guidelines)] = new AttributeValue { S = guidelines };
        _attributes[nameof(SubGroupings)] = new AttributeValue {  = subGroupings };
        _attributes[nameof(StartDateTime)] = new AttributeValue { S = $"{startDateTime}" };
        _attributes[nameof(EndDateTime)] = new AttributeValue { S = $"{endDateTime}" };
    }
    public Grouping(Dictionary<string, AttributeValue> dictionary) : base(dictionary) {}
    public string GroupingName => _attributes[nameof(PartitionKey)].S.Remove(0, PartitionKeyValuePrefix.Length);
    public string Guidelines => _attributes[nameof(Guidelines)].S;
    public List<string> SubGroupings => _attributes[nameof(SubGroupings)].SS;
    public DateTime StartDateTime => DateTime.Parse(_attributes[nameof(StartDateTime)].S);
    public DateTime EndDateTime => DateTime.Parse(_attributes[nameof(EndDateTime)].S);

    public class SubGrouping : BaseEntity
    {
        public string Name { get; init; }
        public List<string> Submissions { get; init; }
    }
}