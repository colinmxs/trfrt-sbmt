using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Festival : BaseEntity
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

    internal void Update(string name, string guidelines, DateTime startDateTime, DateTime endDateTime)
    {
        _attributes[nameof(Name)] = new AttributeValue { S = name };
        _attributes[nameof(Guidelines)] = new AttributeValue { S = guidelines };
        _attributes[nameof(StartDateTime)] = new AttributeValue { S = $"{startDateTime}" };
        _attributes[nameof(EndDateTime)] = new AttributeValue { S = $"{endDateTime}" };
    }

    public override async Task DeleteAsync(IAmazonDynamoDB db, string tableName)
    {
        IEnumerable<Fort> forts = await GetAllFortsAsync(db, tableName);

        await db.DeleteItemAsync(new DeleteItemRequest(tableName, new Dictionary<string, AttributeValue>()
        {
            [nameof(PartitionKey)] = new AttributeValue(PartitionKey),
            [nameof(SortKey)] = new AttributeValue(SortKey)
        }));

        foreach (var fort in forts)
        {
            await fort.DeleteAsync(db, tableName);
        }
    }

    private async Task<IEnumerable<Fort>> GetAllFortsAsync(IAmazonDynamoDB db, string tableName)
    {
        var pkSymbol = ":pk";
        var skSymbol = ":sk";
        List<Fort> forts = new List<Fort>();
        QueryResponse? fortsResult = null;
        do
        {
            fortsResult = await db.QueryAsync(new QueryRequest(tableName)
            {
                KeyConditionExpression = $"{nameof(PartitionKey)} = {pkSymbol} and begins_with({nameof(SortKey)}, {skSymbol})",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    { pkSymbol, new AttributeValue(PartitionKey) },
                    { skSymbol, new AttributeValue(nameof(Fort)) }
                },
                ExclusiveStartKey = fortsResult?.LastEvaluatedKey
            });
            forts.AddRange(fortsResult.Items.Select(i => new Fort(i)));
        } while (fortsResult.LastEvaluatedKey.Count() != 0);
        return forts;
    }
}