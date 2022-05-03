using Amazon.CDK.AWS.DynamoDB;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace TrfrtSbmt.Cdk;

public class DbStack : Stack
{
    public DbStack(Construct scope, string id, StackProps props, string[] replicateRegions) : base(scope, id, props)
    {
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
    }
}
