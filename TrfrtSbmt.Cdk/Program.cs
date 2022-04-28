var app = new App(null);
Tags.Of(app).Add("Owner", "smith.colin00@gmail.com");
Tags.Of(app).Add("Application", "Treefort Submit Api");

_ = new ApiStack(app, "TreefortSubmitApiStack", new ApiStack.ApiStackProps());

app.Synth();