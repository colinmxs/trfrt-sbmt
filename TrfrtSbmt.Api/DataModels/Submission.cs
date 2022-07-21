namespace TrfrtSbmt.Api.DataModels;

using Amazon.DynamoDBv2.Model;

public class Submission : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Submission)}-";
    public string Statement => _attributes[nameof(Statement)].S;
    public string SubmissionDate => _attributes[nameof(SubmissionDate)].S;
    public string FestivalId => _attributes[nameof(FestivalId)].S;
    public string City => _attributes[nameof(City)].S;
    public string Country => _attributes[nameof(Country)].S;
    public string Description => _attributes[nameof(Description)].S;
    public string Image => _attributes[nameof(Image)].S;
    public string Website => _attributes[nameof(Website)].S;
    public IEnumerable<string> Genres => _attributes[nameof(Genres)].SS;
    public string Links => _attributes[nameof(Links)].S;
    public string ContactInfo => _attributes[nameof(ContactInfo)].S;
    public string Location => _attributes[nameof(Location)].S;
    public IEnumerable<Label> Labels => GetLabels();
    public string State => GetState();
    public bool? Approved => GetApproved();
    public string? ReviewedBy => GetApprovedBy();
    public bool NeedsReview => GetNeedsReview();

    public Submission(Dictionary<string, AttributeValue> values) : base(values) { }

    public Submission(string? festivalId,
                      string fortId,
                      string createdBy,
                      string name,
                      string state,
                      string city,
                      string country,
                      string description,
                      string image,
                      string website,
                      IEnumerable<string>? genres,
                      string statement,
                      string links,
                      string contactInfo) : base(fortId, name, name, createdBy)
    {
        _attributes[nameof(NeedsReview)] = new AttributeValue { BOOL = true };
        _attributes[nameof(SubmissionDate)] = new AttributeValue { S = DateTime.UtcNow.ToString("s") };
        _attributes[nameof(FestivalId)] = new AttributeValue { S = festivalId };
        _attributes[nameof(State)] = new AttributeValue { S = state };
        _attributes[nameof(City)] = new AttributeValue { S = city };
        _attributes[nameof(Country)] = new AttributeValue { S = country };
        _attributes[nameof(Description)] = new AttributeValue { S = description };
        _attributes[nameof(Image)] = new AttributeValue { S = image };
        _attributes[nameof(Website)] = new AttributeValue { S = website };
        _attributes[nameof(Links)] = new AttributeValue { S = links };
        _attributes[nameof(ContactInfo)] = new AttributeValue { S = contactInfo };
        _attributes[nameof(Statement)] = new AttributeValue { S = statement };
        _attributes[nameof(Location)] = new AttributeValue { S = $"{Country}{State}{City}" };

        if (genres != null)
            _attributes[nameof(Genres)] = new AttributeValue { SS = genres.ToList() };
    }

    public Submission(string labelId, Dictionary<string, AttributeValue> submission) : base(submission)
    {
        _attributes[nameof(PartitionKey)] = new AttributeValue { S = labelId };
    }
    
    public void Approve(string approver)
    {
        _attributes[nameof(ReviewedBy)] = new AttributeValue { S = approver };
        _attributes.Remove(nameof(NeedsReview));
        _attributes[nameof(Approved)] = new AttributeValue { BOOL = true };
    }

    public void Reject(string approver)
    {
        _attributes[nameof(ReviewedBy)] = new AttributeValue { S = approver };
        _attributes.Remove(nameof(NeedsReview));
        _attributes[nameof(Approved)] = new AttributeValue { BOOL = false };
    }
    
    public void RescindReview()
    {
        _attributes.Remove(nameof(ReviewedBy));
        _attributes[nameof(NeedsReview)] = new AttributeValue { BOOL = true };
        _attributes.Remove(nameof(Approved));
    }

    public void AddLabel(Label label)
    {
        if (!Labels.Any())
        {
            _attributes.Add(nameof(Labels), new AttributeValue { L = new List<AttributeValue> { new AttributeValue { M = label.ToDictionary() } } });
        }
        else
        {
            _attributes[nameof(Labels)].L.Add(new AttributeValue { M = label.ToDictionary() });
        }
    }
    
    public void RemoveLabel(Label? label)
    {
        if (label != null)
        {
            var remainingLabels = Labels.Where(l => l.Id != label.Id).ToList();
            _attributes[nameof(Labels)] = new AttributeValue
            {
                L = remainingLabels.Select(l => new AttributeValue { M = l.ToDictionary() }).ToList()
            };
        }

        if (!Labels.Any())
        {
            _attributes.Remove(nameof(Labels));
        }
    }

    internal void Update(string name, string state, string city, string country, string description, string image, string website, IEnumerable<string>? genres, string statement, string links, string contact)
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
        _attributes[nameof(Location)] = new AttributeValue { S = $"{Country}{State}{City}" };
    }

    private bool GetNeedsReview()
    {
        if(_attributes.TryGetValue(nameof(NeedsReview), out var _))
            return true;
        return false;
    }
    private bool? GetApproved()
    {
        if (_attributes.TryGetValue(nameof(Approved), out var approved))
            return approved.BOOL;
        return false;
    }
    private string? GetApprovedBy()
    {
        if (_attributes.TryGetValue(nameof(ReviewedBy), out var approvedBy))
            return approvedBy.S;
        return null;
    }
    private string GetState()
    {
        if (_attributes.TryGetValue(nameof(State), out var state))
            return state.S;
        return string.Empty;
    }
    private List<Label> GetLabels()
    {
        if (_attributes.TryGetValue(nameof(Labels), out var labels))
            return labels.L.Select(av => new Label(av.M)).ToList();
        return new List<Label>();
    }
    
    private List<string> GetGenres()
    {
        if (_attributes.TryGetValue(nameof(Genres), out var genres))
            return genres.SS;
        return new List<string>();
    }

    public class Label
    {
        public Label(SubmissionLabel label)
        {
            Id = label.PartitionKey;
            Name = label.Name;
            CreatedBy = label.CreatedBy;
        }
        public Label(Dictionary<string, AttributeValue> dict)
        {
            Id = dict[nameof(Id)].S;
            Name = dict[nameof(Name)].S;
            CreatedBy = dict[nameof(CreatedBy)].S;
        }

        internal Dictionary<string, AttributeValue> ToDictionary()
        {
            return new Dictionary<string, AttributeValue>
            {
                [nameof(Id)] = new AttributeValue(Id),
                [nameof(Name)] = new AttributeValue(Name),
                [nameof(CreatedBy)] = new AttributeValue(CreatedBy)
            };
        }

        public string Id { get; init; }
        public string Name { get; init; }
        public string CreatedBy { get; init; }
    }
}