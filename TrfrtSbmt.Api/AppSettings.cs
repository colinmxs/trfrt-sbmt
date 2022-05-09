namespace TrfrtSbmt.Api;

public class AppSettings
{
    public string EnvironmentName { get; init; }
    public Cognito CognitoSettings { get; init; }
    public string BucketName { get; init; }
    public string TableName { get; init; }
    public string SubmissionGrouping { get; init; }

    public AppSettings(ConfigurationManager config)
    {
        var appSettings = config.GetSection("AppSettings");
        CognitoSettings = new Cognito(appSettings["CognitoSettings:ResponseType"], appSettings["CognitoSettings:MetadataAddress"], appSettings["CognitoSettings:ClientId"]);
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        ArgumentNullException.ThrowIfNull(environment, "ASPNETCORE_ENVIRONMENT");
        EnvironmentName = environment;
        BucketName = appSettings["BucketName"];
        TableName = appSettings["TableName"];
        SubmissionGrouping = appSettings["SubmissionGrouping"];
    }
    public record Cognito(string ResponseType, string MetadataAddress, string ClientId);
}

