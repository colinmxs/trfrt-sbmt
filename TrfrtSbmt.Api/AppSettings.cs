namespace TrfrtSbmt.Api;
using System.Collections.Generic;

public class AppSettings
{
    public string EnvironmentName { get; init; }
    public Cognito CognitoSettings { get; init; }

    public AppSettings(ConfigurationManager config)
    {
        config.GetSection("AppSettings").Bind(this);
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        ArgumentNullException.ThrowIfNull(environment, "ASPNETCORE_ENVIRONMENT");
        EnvironmentName = environment;
        CognitoSettings = new Cognito(
            config["Authentication:Cognito:ResponseType"], 
            config["Authentication:Cognito:MetadataAddress"], 
            config["Authentication:Cognito:ClientId"]);
    }
    public record Cognito(string ResponseType, string MetadataAddress, string ClientId);

}

