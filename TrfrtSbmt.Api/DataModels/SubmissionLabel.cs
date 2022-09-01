using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class SubmissionLabel : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(SubmissionLabel)}-";

    public SubmissionLabel(Dictionary<string, AttributeValue> values) : base(values) { }
    public SubmissionLabel(Label label, Submission submission, string createdBy) : base(label.EntityId, submission.EntityId, label.Name, label.Name, createdBy) 
    {
        _attributes[nameof(SubmissionEntityId)] = new AttributeValue(submission.EntityId);
        _attributes[nameof(Name)] = new AttributeValue(submission.Name);
        _attributes[nameof(State)] = new AttributeValue(submission.State);
        _attributes[nameof(City)] = new AttributeValue(submission.City);
        _attributes[nameof(Image)] = new AttributeValue(submission.Image);
    }

    public SubmissionLabel(Submission.Label label, Submission submission) : base(label.Id, submission.EntityId, label.Name, label.Name, label.CreatedBy)
    {
        _attributes[nameof(SubmissionEntityId)] = new AttributeValue(submission.EntityId);
        _attributes[nameof(Name)] = new AttributeValue(submission.Name);
        _attributes[nameof(State)] = new AttributeValue(submission.State);
        _attributes[nameof(City)] = new AttributeValue(submission.City);
        _attributes[nameof(Image)] = new AttributeValue(submission.Image);
    }

    public string SubmissionEntityId => _attributes[nameof(SubmissionEntityId)].S;

    public string State => _attributes[nameof(State)].S;
    public string City => _attributes[nameof(City)].S;
    public string Image => _attributes[nameof(Image)].S;
}
