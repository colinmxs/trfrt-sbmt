namespace TrfrtSbmt.Api.DataModels;

using Amazon.DynamoDBv2.Model;

public class Submission : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Submission)}-";

    public Submission(Dictionary<string, AttributeValue> values) : base(values) { }

    public Submission(string festivalId, string fortId, string createdBy, string name, string state, string city, string country, string description, string image, string website, IEnumerable<string> genres, string statement, string links, string contactInfo) : base(fortId, name, name)
    {
        _attributes[nameof(SubmissionDate)] = new AttributeValue { S = DateTime.UtcNow.ToString() };
        _attributes[nameof(FestivalId)] = new AttributeValue { S = festivalId };
        _attributes[nameof(State)] = new AttributeValue { S = state };
        _attributes[nameof(City)] = new AttributeValue { S = city };
        _attributes[nameof(Country)] = new AttributeValue { S = country };
        _attributes[nameof(Description)] = new AttributeValue { S = description };
        _attributes[nameof(Image)] = new AttributeValue { S = image };
        _attributes[nameof(Website)] = new AttributeValue { S = website };
        _attributes[nameof(Genres)] = new AttributeValue { SS = genres.ToList() };
        _attributes[nameof(Links)] = new AttributeValue { S = links };
        _attributes[nameof(ContactInfo)] = new AttributeValue { S = contactInfo };
        _attributes[nameof(Statement)] = new AttributeValue { S = statement };
    }
    public Submission(string labelId, Dictionary<string, AttributeValue> submission) : base(submission) 
    {
        _attributes[nameof(PartitionKey)] = new AttributeValue { S = labelId };
    }
    public string Statement => _attributes[nameof(Statement)].S;
    public string SubmissionDate => _attributes[nameof(SubmissionDate)].S;

    internal void Update(string name, string state, string city, string country, string description, string image, string website, IEnumerable<string> genres, string statement, string links, string contact)
    {
        _attributes[nameof(Name)] = new AttributeValue { S = name };
        _attributes[nameof(State)] = new AttributeValue { S = state };
        _attributes[nameof(City)] = new AttributeValue { S = city };
        _attributes[nameof(Country)] = new AttributeValue { S = country };
        _attributes[nameof(Description)] = new AttributeValue { S = description };
        _attributes[nameof(Image)] = new AttributeValue { S = image };
        _attributes[nameof(Website)] = new AttributeValue { S = website };
        _attributes[nameof(Genres)] = new AttributeValue { SS = genres.ToList() };
        _attributes[nameof(Links)] = new AttributeValue { S = links };
        _attributes[nameof(ContactInfo)] = new AttributeValue { S = contact };
        _attributes[nameof(Statement)] = new AttributeValue { S = statement };
    }
    public string FestivalId => _attributes[nameof(FestivalId)].S;
    public string State => _attributes[nameof(State)].S;
    public string City => _attributes[nameof(City)].S;
    public string Country => _attributes[nameof(Country)].S;
    public string Description => _attributes[nameof(Description)].S;
    public string Image => _attributes[nameof(Image)].S;
    public string Website => _attributes[nameof(Website)].S;
    public IEnumerable<string> Genres => _attributes[nameof(Genres)].SS;
    public string Links => _attributes[nameof(Links)].S;
    public string ContactInfo => _attributes[nameof(ContactInfo)].S;
}