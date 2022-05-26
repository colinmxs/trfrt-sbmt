using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public abstract class BaseEntity
{
    internal const string Gsi1 = "Gsi1";
    protected readonly Dictionary<string, AttributeValue> _attributes;
    public BaseEntity(Dictionary<string, AttributeValue> values) 
    {
        _attributes = values;
    }
    
    public BaseEntity(string? partitionKey, string sortKey, string searchTerm, string entityType, string? entityId = null)
    {
        _attributes = new Dictionary<string, AttributeValue>
        {
            [nameof(EntityId)] = new AttributeValue(entityId ?? Guid.NewGuid().ToString()),
            [nameof(PartitionKey)] = new AttributeValue { S = partitionKey },
            [nameof(SortKey)] = new AttributeValue { S = sortKey },
            [nameof(SearchTerm)] = new AttributeValue { S = searchTerm.ToUpperInvariant() },
            [nameof(EntityType)] = new AttributeValue { S = entityType }
        };

    }
    protected internal string EntityId => _attributes[nameof(EntityId)].S;
    protected internal string PartitionKey => _attributes[nameof(PartitionKey)].S;
    protected internal string SortKey => _attributes[nameof(SortKey)].S;
    protected string EntityType => _attributes[nameof(EntityType)].S;
    protected string SearchTerm => _attributes[nameof(SearchTerm)].S;

    internal Dictionary<string, AttributeValue> ToDictionary()
    {
        return new Dictionary<string, AttributeValue>(_attributes);
    }
}