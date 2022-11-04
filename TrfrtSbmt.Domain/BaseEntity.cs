namespace TrfrtSbmt.Domain;

using Amazon.DynamoDBv2.Model;

public abstract class BaseEntity
{
    public abstract string SortKeyPrefix { get; }

    public const string EntityIdIndex = "EntityIdIndex";
    protected readonly Dictionary<string, AttributeValue> _attributes;

    public BaseEntity(Dictionary<string, AttributeValue> values)
    {
        _attributes = values;
    }

    /// this ctor generates and entityid which it uses for the sortkey
    public BaseEntity(string partitionKey, string name, string searchTerm, string createdBy)
    {
        var typeName = GetType().Name;
        var id = Guid.NewGuid().ToString();
        _attributes = new Dictionary<string, AttributeValue>
        {
            [nameof(EntityId)] = new AttributeValue(id),
            [nameof(PartitionKey)] = new AttributeValue { S = partitionKey },
            [nameof(SortKey)] = new AttributeValue { S = $"{SortKeyPrefix}{id}" },
            [nameof(Name)] = new AttributeValue { S = name },
            [nameof(SearchTerm)] = new AttributeValue { S = searchTerm.ToUpperInvariant() },
            [nameof(EntityType)] = new AttributeValue { S = typeName },
            [nameof(CreatedBy)] = new AttributeValue { S = createdBy }
        };
    }

    public BaseEntity(string partitionKey, string sortKeySuffix, string name, string searchTerm, string createdBy)
    {
        var typeName = GetType().Name;
        var id = Guid.NewGuid().ToString();
        _attributes = new Dictionary<string, AttributeValue>
        {
            [nameof(EntityId)] = new AttributeValue(id),
            [nameof(PartitionKey)] = new AttributeValue { S = partitionKey },
            [nameof(SortKey)] = new AttributeValue { S = $"{SortKeyPrefix}{sortKeySuffix}" },
            [nameof(Name)] = new AttributeValue { S = name },
            [nameof(SearchTerm)] = new AttributeValue { S = searchTerm.ToUpperInvariant() },
            [nameof(EntityType)] = new AttributeValue { S = typeName },
            [nameof(CreatedBy)] = new AttributeValue { S = createdBy }
        };
    }


    public BaseEntity(string name, string searchTerm, string createdBy)
    {
        var id = Guid.NewGuid().ToString();
        var typeName = GetType().Name;
        _attributes = new Dictionary<string, AttributeValue>
        {
            [nameof(EntityId)] = new AttributeValue(id),
            [nameof(PartitionKey)] = new AttributeValue { S = id },
            [nameof(SortKey)] = new AttributeValue { S = $"{SortKeyPrefix}{id}" },
            [nameof(Name)] = new AttributeValue { S = name },
            [nameof(SearchTerm)] = new AttributeValue { S = searchTerm.ToUpperInvariant() },
            [nameof(EntityType)] = new AttributeValue { S = typeName },
            [nameof(CreatedBy)] = new AttributeValue { S = createdBy }
        };
    }
    public string CreatedBy => _attributes[nameof(CreatedBy)].S;
    public string Name => _attributes[nameof(Name)].S;
    public string EntityId => _attributes[nameof(EntityId)].S;
    public string PartitionKey => _attributes[nameof(PartitionKey)].S;
    public string SortKey => _attributes[nameof(SortKey)].S;
    public string EntityType => _attributes[nameof(EntityType)].S;
    public string SearchTerm => _attributes[nameof(SearchTerm)].S;

    public Dictionary<string, AttributeValue> ToDictionary()
    {
        return new Dictionary<string, AttributeValue>(_attributes);
    }
}