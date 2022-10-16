using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SES.Actions;

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

var api = new RegionalStack(app, $"{envPrefix}TrfrtSbmt-ApiStack-{region}", new RegionalStack.RegionalStackProps
{
    Env = new Amazon.CDK.Environment { Region = region, Account = accountId },
    Name = $"TrfrtSbmt-{region}",
    CertId = certId,
    Region = region,
    EnvironmentName = env,
    EnvironmentPrefix = envPrefix    
});

app.Synth();