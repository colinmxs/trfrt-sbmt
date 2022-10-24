namespace TrfrtSbmt.Api.DataModels;

using Amazon.DynamoDBv2.Model;

public class SubmissionRank : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(SubmissionRank)}-";

    public SubmissionRank(Dictionary<string, AttributeValue> values) : base(values) { }
    
    public SubmissionRank(Submission submission, decimal rank, decimal count) : base(submission.EntityId, submission.Name, nameof(SubmissionRank), "SYSTEM")
    {
        _attributes[nameof(NumberOfVotes)] = new AttributeValue { N = count.ToString() };
        _attributes[nameof(SubmissionEntityId)] = new AttributeValue(submission.EntityId);
        _attributes[nameof(State)] = new AttributeValue(submission.State);
        _attributes[nameof(City)] = new AttributeValue(submission.City);
        _attributes[nameof(Country)] = new AttributeValue(submission.Country);
        _attributes[nameof(Image)] = new AttributeValue(submission.Image);
        _attributes[nameof(FortId)] = new AttributeValue(submission.FortId);
        _attributes[nameof(Location)] = new AttributeValue { S = $"{Country}{State}{City}" };
        _attributes[nameof(Rank)] = new AttributeValue { N = rank.ToString() };
        _attributes[nameof(LocationRank)] = new AttributeValue { S = $"{Country}{State}{City}-{rank.ToString()}" };
    }

    public string NumberOfVotes => _attributes[nameof(NumberOfVotes)].N.ToString();
    public string SubmissionEntityId => _attributes[nameof(SubmissionEntityId)].S;
    public string State => _attributes[nameof(State)].S;
    public string City => _attributes[nameof(City)].S;
    public string Country => _attributes[nameof(Country)].S;
    public string Image => _attributes[nameof(Image)].S;
    public string FortId => _attributes[nameof(FortId)].S;
    public string Location => _attributes[nameof(Location)].S;
    public string Rank => _attributes[nameof(Rank)].N.ToString();
    public string LocationRank => _attributes[nameof(LocationRank)].S;
}
