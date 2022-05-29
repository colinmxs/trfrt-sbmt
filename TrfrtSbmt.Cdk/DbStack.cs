using Amazon.CDK.AWS.DynamoDB;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace TrfrtSbmt.Cdk;

public class DbStack : Stack
{
    public DbStack(Construct scope, string id, StackProps props, string[] replicateRegions) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");
        var table = new Table(this, "DynamoTable", new TableProps
        {
            BillingMode = BillingMode.PAY_PER_REQUEST,
            PartitionKey = new Attribute
            {
                Name = "PartitionKey",
                Type = AttributeType.STRING
            },
            SortKey = new Attribute
            {
                Name = "SortKey",
                Type = AttributeType.STRING
            },
            RemovalPolicy = RemovalPolicy.DESTROY,
            TableName = $"Submissions",
            ReplicationRegions = replicateRegions
        });

        Amazon.CDK.Tags.Of(table).Add("Name", "Submissions");
        Amazon.CDK.Tags.Of(table).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "Gsi1",
            PartitionKey = new Attribute
            {
                Name = "SearchTerm",
                Type = AttributeType.STRING
            },
            SortKey = new Attribute
            {
                Name = "EntityType",
                Type = AttributeType.STRING
            }
        });
        
        table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "EntityIdIndex",
            PartitionKey = new Attribute
            {
                Name = "EntityId",
                Type = AttributeType.STRING
            }
        });

        var testTable = new Table(this, "DynamoTestTable", new TableProps
        {
            BillingMode = BillingMode.PAY_PER_REQUEST,
            PartitionKey = new Attribute
            {
                Name = "PartitionKey",
                Type = AttributeType.STRING
            },
            SortKey = new Attribute
            {
                Name = "SortKey",
                Type = AttributeType.STRING
            },
            RemovalPolicy = RemovalPolicy.DESTROY,
            TableName = $"Submissions-Tests"
        });

        Amazon.CDK.Tags.Of(table).Add("Name", "Submissions-Tests");
        Amazon.CDK.Tags.Of(table).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        testTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "Gsi1",
            PartitionKey = new Attribute
            {
                Name = "SearchTerm",
                Type = AttributeType.STRING
            },
            SortKey = new Attribute
            {
                Name = "EntityType",
                Type = AttributeType.STRING
            }
        });

        testTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "EntityIdIndex",
            PartitionKey = new Attribute
            {
                Name = "EntityId",
                Type = AttributeType.STRING
            }
        });
    }
}
