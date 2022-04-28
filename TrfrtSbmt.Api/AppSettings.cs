namespace TrfrtSbmt.Api;
using System.Collections.Generic;

public class AppSettings
{
    public AppSettings(ConfigurationManager config)
    {
        config.GetSection("AppSettings").Bind(this);
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        ArgumentNullException.ThrowIfNull(environment, "ASPNETCORE_ENVIRONMENT");
        EnvironmentName = environment;
    }
    public string EnvironmentName { get; set; }
}
