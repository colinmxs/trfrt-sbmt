using Amazon.CDK.AWS.DynamoDB;

namespace TrfrtSbmt.Cdk.Stacks;
public class DbStackProps : StackProps
{
    public string EnvironmentName { get; init; } = "Development";
    public string EnvironmentPrefix { get; init; } = "Development-";
}
public class DbStack : Stack
{
    public Table Table { get; }
    //public Table TestTable { get; }
    public DbStack(Construct scope, string id, DbStackProps props, string[]? replicateRegions = null) : base(scope, id, props)
    {
        Amazon.CDK.Tags.Of(this).Add("Billing", "Treefort");
        
        Table = new SubmissionsTable(this, "DynamoTable", new SubmissionsTableProps 
        {
            EnvironmentName = props.EnvironmentName,
            RemovalPolicy = RemovalPolicy.RETAIN,
            TableName = "Submissions"
        }).Table;

        //TestTable = new SubmissionsTable(this, "TestDynamoTable", new SubmissionsTableProps
        //{
        //    EnvironmentName = props.EnvironmentName,
        //    RemovalPolicy = RemovalPolicy.DESTROY,
        //    TableName = "Submissions-Tests"
        //}).Table;
    }
}
