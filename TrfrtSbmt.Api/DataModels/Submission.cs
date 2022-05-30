namespace TrfrtSbmt.Api.DataModels;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

public class Submission : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Submission)}-";

    public Submission(Dictionary<string, AttributeValue> values) : base(values) { }

    public Submission(string fortId, string name, string state, string city, string country, string description, string image, string website, string genre, string links, string contactInfo) : base(fortId, name, name)
    {
        _attributes[nameof(State)] = new AttributeValue { S = state };
        _attributes[nameof(City)] = new AttributeValue { S = city };
        _attributes[nameof(Country)] = new AttributeValue { S = country };
        _attributes[nameof(Description)] = new AttributeValue { S = description };
        _attributes[nameof(Image)] = new AttributeValue { S = image };
        _attributes[nameof(Website)] = new AttributeValue { S = website };
        _attributes[nameof(Genre)] = new AttributeValue { S = genre };
        _attributes[nameof(Links)] = new AttributeValue { S = links };
        _attributes[nameof(ContactInfo)] = new AttributeValue { S = contactInfo };
    }

    public string State => _attributes[nameof(State)].S;
    public string City => _attributes[nameof(City)].S;
    public string Country => _attributes[nameof(Country)].S;
    public string Description => _attributes[nameof(Description)].S;
    public string Image => _attributes[nameof(Image)].S;
    public string Website => _attributes[nameof(Website)].S;
    public string Genre => _attributes[nameof(Genre)].S;
    public string Links => _attributes[nameof(Links)].S;
    public string ContactInfo => _attributes[nameof(ContactInfo)].S;


    public override Task DeleteAsync(IAmazonDynamoDB db, string tableName)
    {
        throw new NotImplementedException();
    }
}