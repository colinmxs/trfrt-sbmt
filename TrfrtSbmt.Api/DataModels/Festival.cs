using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public partial class Festival : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Festival)}-";
    public Festival(Dictionary<string, AttributeValue> dictionary) : base(dictionary) { }
    public Festival(string name, string guidelines, DateTime startDateTime, DateTime endDateTime) : base(name, nameof(Festival)) 
    {
        _attributes[nameof(Guidelines)] = new AttributeValue { S = guidelines };        
        _attributes[nameof(StartDateTime)] = new AttributeValue { S = $"{startDateTime}" };
        _attributes[nameof(EndDateTime)] = new AttributeValue { S = $"{endDateTime}" };
    }
    public string Guidelines => _attributes[nameof(Guidelines)].S;
    public DateTime StartDateTime => DateTime.Parse(_attributes[nameof(StartDateTime)].S);
    public DateTime EndDateTime => DateTime.Parse(_attributes[nameof(EndDateTime)].S);
}