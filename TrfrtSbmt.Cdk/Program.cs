using Amazon.CDK.AWS.IAM;

var app = new App(null);
var region = "us-west-2";
Tags.Of(app).Add("Owner", "smith.colin00@gmail.com");
Tags.Of(app).Add("Application", "Submit Api");
Tags.Of(app).Add("Billing", "Treefort");
var accountId = (string)app.Node.TryGetContext("accountid");

var regionConfig = (Dictionary<string, object>)app.Node.TryGetContext(region);
var certId = (string)regionConfig["sslcertid"];
var api = new ApiStack(app, $"TrfrtSbmt-ApiStack-{region}", new ApiStack.ApiStackProps
{
    Env = new Amazon.CDK.Environment { Region = region, Account = accountId },
    Name = $"TrfrtSbmt-{region}",
    CertId = certId,
    Region = region,
});

var dbs = new DbStack(app, "TrfrtSbmt-DbStack", new StackProps
{
    Env = new Amazon.CDK.Environment { Region = "us-west-2", Account = accountId }
});

var iam = new IamStack(app, "TrfrtSbmt-IamStack", new IamStack.IamStackProps
{
    Env = new Amazon.CDK.Environment { Region = "us-west-2", Account = accountId },
    Role = api.LambdaExecutionRole,
    Table = dbs.Table,
    TestTable = dbs.TestTable
});


app.Synth();