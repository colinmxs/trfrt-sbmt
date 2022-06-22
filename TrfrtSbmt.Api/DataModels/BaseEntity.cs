using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public abstract class BaseEntity
{
    protected abstract string SortKeyPrefix { get; }
    
    internal const string EntityIdIndex = "EntityIdIndex";
    protected readonly Dictionary<string, AttributeValue> _attributes;
    public BaseEntity(Dictionary<string, AttributeValue> values) 
    {
        _attributes = values;
    }
    
    public BaseEntity(string partitionKey, string name, string searchTerm)
    {
        var typeName = GetType().Name;
        _attributes = new Dictionary<string, AttributeValue>
        {
            [nameof(EntityId)] = new AttributeValue(Guid.NewGuid().ToString()),
            [nameof(PartitionKey)] = new AttributeValue { S = partitionKey },
            [nameof(SortKey)] = new AttributeValue { S = $"{SortKeyPrefix}{name}" },
            [nameof(Name)] = new AttributeValue { S = name },
            [nameof(SearchTerm)] = new AttributeValue { S = searchTerm.ToUpperInvariant() },
            [nameof(EntityType)] = new AttributeValue { S = typeName }
        };
    }
    public BaseEntity(string name, string searchTerm)
    {
        var id = Guid.NewGuid().ToString();
        var typeName = GetType().Name;
        _attributes = new Dictionary<string, AttributeValue>
        {
            [nameof(EntityId)] = new AttributeValue(id),
            [nameof(PartitionKey)] = new AttributeValue { S = id },
            [nameof(SortKey)] = new AttributeValue { S = $"{SortKeyPrefix}{name}" },
            [nameof(Name)] = new AttributeValue { S = name },
            [nameof(SearchTerm)] = new AttributeValue { S = searchTerm.ToUpperInvariant() },
            [nameof(EntityType)] = new AttributeValue { S = typeName }
        };
    }

    public string Name => _attributes[nameof(Name)].S;
    protected internal string EntityId => _attributes[nameof(EntityId)].S;
    protected internal string PartitionKey => _attributes[nameof(PartitionKey)].S;
    protected internal string SortKey => _attributes[nameof(SortKey)].S;
    protected internal string EntityType => _attributes[nameof(EntityType)].S;
    protected internal string SearchTerm => _attributes[nameof(SearchTerm)].S;

    internal Dictionary<string, AttributeValue> ToDictionary()
    {
        return new Dictionary<string, AttributeValue>(_attributes);
    }
}