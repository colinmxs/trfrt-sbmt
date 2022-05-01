namespace TrfrtSbmt.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
public static class Auth
{
    public static AuthenticationBuilder ConfigureCognitoAuth(this IServiceCollection services, AppSettings appSettings, string scheme = JwtBearerDefaults.AuthenticationScheme)
    {
        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(appSettings.CognitoSettings.MetadataAddress, new OpenIdConnectConfigurationRetriever());
        var config = configManager.GetConfigurationAsync(CancellationToken.None).Result;
        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true,
            IssuerSigningKeys = config.SigningKeys
        };
        return services
            .AddAuthentication(scheme)
            .AddJwtBearer(scheme, opts =>
            {
                opts.TokenValidationParameters = validationParameters;
            });
    }
}
