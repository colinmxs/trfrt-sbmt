// specify regions to deploy to
var regions = new string[]
{    
    "us-west-2"
};

var app = new App(null);
Tags.Of(app).Add("Owner", "smith.colin00@gmail.com");
Tags.Of(app).Add("Application", "Treefort Submit Api");

foreach (var region in regions)
{
    var otherRegions = regions.Except(new List<string> { region });
    _ = new ApiStack(app, "TreefortSubmitApiStack", new ApiStack.ApiStackProps
    {
        Env = new Amazon.CDK.Environment { Region = region },
        Name = "TreefortSubmitApi",
        Region = region,
        OtherRegions = otherRegions.ToArray()
    });
}

app.Synth();