var app = new App(null);
Tags.Of(app).Add("Owner", "smith.colin00@gmail.com");
Tags.Of(app).Add("Application", "Submit Api");
Tags.Of(app).Add("Billing", "Treefort");

var accountId = (string)app.Node.TryGetContext("accountid");
var env = (string)app.Node.TryGetContext("asp_env");
var envSuffix = $"-{env}";
var globalConfig = (Dictionary<string, object>)app.Node.TryGetContext("global");
var globalCertId = (string)globalConfig["sslcertid"];

foreach (var region in new[] {"us-west-2", "us-west-1" })
{
    var regionConfig = (Dictionary<string, object>)app.Node.TryGetContext(region);
    var regionCertId = (string)regionConfig["sslcertid"];
    var api = new RegionalStack(app, $"TrfrtSbmt-ApiStack-{region}{envSuffix}", new RegionalStack.RegionalStackProps
    {
        Env = new Amazon.CDK.Environment { Region = region, Account = accountId },
        RegionalCertId = regionCertId,
        GlobalCertId = globalCertId,
        Region = region,
        PrimaryRegion = "us-west-2",
        EnvironmentName = env,
        EnvironmentSuffix = envSuffix    
    });
}


app.Synth();