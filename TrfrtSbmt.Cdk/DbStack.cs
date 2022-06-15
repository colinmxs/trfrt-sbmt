using Amazon.CDK.AWS.DynamoDB;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace TrfrtSbmt.Cdk;

public class DbStack : Stack
{
    public Table Table { get; }
    public Table TestTable { get; }
    public DbStack(Construct scope, string id, StackProps props, string[]? replicateRegions = null) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");
        Table = new Table(this, "DynamoTable", new TableProps
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
            //ReplicationRegions = replicateRegions
        });
        
        Amazon.CDK.Tags.Of(Table).Add("Name", "Submissions");
        Amazon.CDK.Tags.Of(Table).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps 
        {
            IndexName = "StateIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "State",
                Type = AttributeType.STRING
            }            
        });

        Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "RankIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "Rank",
                Type = AttributeType.STRING
            }
        });

        Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "StateRankIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "StateRank",
                Type = AttributeType.STRING
            }
        });

        Table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "SubmissionDateIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "SubmissionDate",
                Type = AttributeType.STRING
            }
        });

        Table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "SearchTermIndex",
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

        Table.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "EntityIdIndex",
            PartitionKey = new Attribute
            {
                Name = "EntityId",
                Type = AttributeType.STRING
            }
        });

        TestTable = new Table(this, "DynamoTestTable", new TableProps
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

        Amazon.CDK.Tags.Of(TestTable).Add("Name", "Submissions-Tests");
        Amazon.CDK.Tags.Of(TestTable).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

        TestTable.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "StateIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "State",
                Type = AttributeType.STRING
            }
        });

        TestTable.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "RankIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "Rank",
                Type = AttributeType.STRING
            }
        });

        TestTable.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "StateRankIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "StateRank",
                Type = AttributeType.STRING
            }
        });

        TestTable.AddLocalSecondaryIndex(new LocalSecondaryIndexProps
        {
            IndexName = "SubmissionDateIndex",
            ProjectionType = ProjectionType.ALL,
            SortKey = new Attribute
            {
                Name = "SubmissionDate",
                Type = AttributeType.STRING
            }
        });

        

        TestTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
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
