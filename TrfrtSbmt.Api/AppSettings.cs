namespace TrfrtSbmt.Api;

public class AppSettings
{
    public string EnvironmentName { get; init; } = "Development";
    public Cognito? CognitoSettings { get; init; } = null;
    public string BucketName { get; init; } = string.Empty;
    public string TableName { get; init; } = string.Empty;
    public string SubmissionGrouping { get; init; } = string.Empty;
    public string FromEmailAddress { get; init; } = string.Empty;

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
    public AppSettings() { }
    public record Cognito(string ResponseType, string MetadataAddress, string ClientId);
}

