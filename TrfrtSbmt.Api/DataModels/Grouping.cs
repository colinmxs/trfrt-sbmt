using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Grouping : BaseEntity
{
    private const string SortKeyValuePrefix = "Grouping-";
    public Grouping(Dictionary<string, AttributeValue> dictionary) : base(dictionary) { }
    public Grouping(string name, string guidelines, DateTime startDateTime, DateTime endDateTime, string entityId) : base(entityId, $"{SortKeyValuePrefix}{name}", nameof(Grouping), nameof(Grouping), entityId)
    {
        _attributes[nameof(Guidelines)] = new AttributeValue { S = guidelines };
        //_attributes[nameof(SubGroupings)] = new AttributeValue { L = new List<AttributeValue>(subGroupings.Select(s => new AttributeValue 
        //{
        //    M = new Dictionary<string, AttributeValue> 
        //    {
        //        ["Id"] = new AttributeValue { S = Guid.NewGuid().ToString() },
        //        ["Name"] = new AttributeValue { S = s }
        //    }   
        //})) };
        _attributes[nameof(Name)] = new AttributeValue { S = name };
        _attributes[nameof(StartDateTime)] = new AttributeValue { S = $"{startDateTime}" };
        _attributes[nameof(EndDateTime)] = new AttributeValue { S = $"{endDateTime}" };
    }
    public string Name => _attributes[nameof(Name)].S;
    public string Guidelines => _attributes[nameof(Guidelines)].S;
    public DateTime StartDateTime => DateTime.Parse(_attributes[nameof(StartDateTime)].S);
    public DateTime EndDateTime => DateTime.Parse(_attributes[nameof(EndDateTime)].S);

    public class SubGrouping : BaseEntity
    {
        internal const string SortKeyValuePrefix = "SubGrouping-";
        public SubGrouping(Dictionary<string, AttributeValue> dictionary) : base(dictionary) { }
        public SubGrouping(string name, string groupingId) : base(groupingId, $"{SortKeyValuePrefix}{name}", name, nameof(SubGrouping))
        {
            _attributes[nameof(Name)] = new AttributeValue { S = name };
        }
        public string Name => _attributes[nameof(Name)].S;
    }
}