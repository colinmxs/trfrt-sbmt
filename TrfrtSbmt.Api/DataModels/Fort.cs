using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace TrfrtSbmt.Api.DataModels;

public class Fort : BaseEntity
{
    protected override string SortKeyPrefix => $"{nameof(Fort)}-";
        
    public Fort(Dictionary<string, AttributeValue> dictionary) : base(dictionary) { }
    public Fort(string festivalId, string name) : base(festivalId, name, name) { }
    public override async Task DeleteAsync(IAmazonDynamoDB db, string tableName)
    {
        await db.DeleteItemAsync(new DeleteItemRequest(tableName, new Dictionary<string, AttributeValue>()
        {
            [nameof(PartitionKey)] = new AttributeValue(PartitionKey),
            [nameof(SortKey)] = new AttributeValue(SortKey)
        }));
        
        var submissionsResult = await db.QueryAsync(new QueryRequest(tableName)
        {
            KeyConditionExpression = $"{nameof(PartitionKey)} = :pk",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
            {
                {":pk", new AttributeValue(EntityId)}
            }
        });

        foreach (var item in submissionsResult.Items)
        {
            await db.DeleteItemAsync(new DeleteItemRequest(tableName, new Dictionary<string, AttributeValue>()
            {
                [nameof(PartitionKey)] = new AttributeValue(item[nameof(PartitionKey)].S),
                [nameof(SortKey)] = new AttributeValue(item[nameof(SortKey)].S)
            }));
        }
    }
}
