﻿// specify regions to deploy to
var regions = new string[]
{  
    "us-west-2"
};

var app = new App(null);
Tags.Of(app).Add("Owner", "smith.colin00@gmail.com");
Tags.Of(app).Add("Application", "Submit Api");

var accountId = (string)app.Node.TryGetContext("accountid");
_ = new DbStack(app, "DbStack", new StackProps
{
    Env = new Amazon.CDK.Environment { Region = "us-west-2", Account = accountId },
}, regions.Except(new List<string> { "us-west-2" }).ToArray());
foreach (var region in regions)
{
    var regionConfig = (Dictionary<string, object>)app.Node.TryGetContext(region);
    var certId = (string)regionConfig["sslcertid"];
    var otherRegions = regions.Except(new List<string> { region });
    _ = new ApiStack(app, $"SubmitApiStack-{region}", new ApiStack.ApiStackProps
    {
        Env = new Amazon.CDK.Environment { Region = region, Account = accountId },
        Name = $"SubmitApiStack-{region}",
        CertId = certId,
        Region = region,
        OtherRegions = otherRegions.ToArray()
    });
}

app.Synth();