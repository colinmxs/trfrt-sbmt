namespace TrfrtSbmt.Domain;

using Amazon.DynamoDBv2.Model;

public sealed class Festival : BaseEntity
{
    public override string SortKeyPrefix => $"{nameof(Festival)}-";
    public Festival(Dictionary<string, AttributeValue> dictionary) : base(dictionary) { }
    public Festival(bool isActive, string name, string username, string guidelines, DateTime startDateTime, DateTime endDateTime) : base(name, nameof(Festival), username)
    {
        _attributes[nameof(Guidelines)] = new AttributeValue { S = guidelines };
        _attributes[nameof(IsActive)] = new AttributeValue { BOOL = isActive };
        _attributes[nameof(StartDateTime)] = new AttributeValue { S = $"{startDateTime}" };
        _attributes[nameof(EndDateTime)] = new AttributeValue { S = $"{endDateTime}" };
    }
    public bool IsActive => _attributes[nameof(IsActive)].BOOL;
    public string Guidelines => _attributes[nameof(Guidelines)].S;
    public DateTime StartDateTime => DateTime.Parse(_attributes[nameof(StartDateTime)].S);
    public DateTime EndDateTime => DateTime.Parse(_attributes[nameof(EndDateTime)].S);

    public void Update(bool isActive, string name, string guidelines, DateTime startDateTime, DateTime endDateTime)
    {
        _attributes[nameof(IsActive)] = new AttributeValue { BOOL = isActive };
        _attributes[nameof(Name)] = new AttributeValue { S = name };
        _attributes[nameof(Guidelines)] = new AttributeValue { S = guidelines };
        _attributes[nameof(StartDateTime)] = new AttributeValue { S = $"{startDateTime}" };
        _attributes[nameof(EndDateTime)] = new AttributeValue { S = $"{endDateTime}" };
    }
}