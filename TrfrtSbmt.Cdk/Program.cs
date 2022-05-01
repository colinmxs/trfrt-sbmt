using Amazon.CDK.AWS.DynamoDB;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

// specify regions to deploy to
var regions = new string[] 
{
    "us-west-1",
    "us-west-2"
};

var app = new App(null);
Tags.Of(app).Add("Owner", "smith.colin00@gmail.com");
Tags.Of(app).Add("Application", "Treefort Submit Api");

var table = new Table(app, "DynamoTable", new TableProps
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
    ReplicationRegions = regions
});

Tags.Of(table).Add("Name", "Submissions");
Tags.Of(table).Add("Last Updated", DateTimeOffset.UtcNow.ToString());

foreach (var region in regions)
{
    _ = new ApiStack(app, "TreefortSubmitApiStack", new ApiStack.ApiStackProps
    {
        Env = new Amazon.CDK.Environment { Region = region }
    });
}

app.Synth();