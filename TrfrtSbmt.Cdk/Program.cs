var app = new App(null);
Tags.Of(app).Add("Owner", "smith.colin00@gmail.com");
Tags.Of(app).Add("Application", "Submit Api");
Tags.Of(app).Add("Billing", "Treefort");

var region = "us-west-2";
var accountId = (string)app.Node.TryGetContext("accountid");
var env = (string)app.Node.TryGetContext("asp_env");
var envPrefix = env != "Production" ? $"{env}-" : string.Empty;
var regionConfig = (Dictionary<string, object>)app.Node.TryGetContext(region);
var certId = (string)regionConfig["sslcertid"];
var api = new ApiStack(app, $"{envPrefix}TrfrtSbmt-ApiStack-{region}", new ApiStack.ApiStackProps
{
    Env = new Amazon.CDK.Environment { Region = region, Account = accountId },
    Name = $"TrfrtSbmt-{region}",
    CertId = certId,
    Region = region,
    EnvironmentName = env,
    EnvironmentPrefix = envPrefix
});
if (true)
{
    var dbs = new DbStack(app, $"{envPrefix}TrfrtSbmt-DbStack", new DbStackProps
    {
        Env = new Amazon.CDK.Environment { Region = "us-west-2", Account = accountId },
        EnvironmentName = env,
        EnvironmentPrefix = envPrefix
    });

    var s3 = new S3Stack(app, $"{envPrefix}TrfrtSbmt-S3Stack", new S3Stack.S3StackProps() 
    {    
        EnvironmentName = env,
        EnvironmentPrefix = envPrefix
    });

    var iam = new IamStack(app, $"{envPrefix}TrfrtSbmt-IamStack", new IamStack.IamStackProps
    {
        Env = new Amazon.CDK.Environment { Region = "us-west-2", Account = accountId },
        Role = api.LambdaExecutionRole,
        Table = dbs.Table,
        TestTable = dbs.TestTable,
        Bucket = s3.Bucket,
        EnvironmentName = env,
        EnvironmentPrefix = envPrefix
    });
}

app.Synth();