namespace TrfrtSbmt.Api.Utils.DiscordWebhooks;
using System.Text.Json.Serialization;

public class EmbedFooter
{
    [JsonPropertyName("text")]
    /// <summary>
    /// Footer text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon_url")]
    /// <summary>
    /// Footer icon
    /// </summary>
    public string? IconUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("proxy_icon_url")]
    /// <summary>
    /// Footer icon proxy
    /// </summary>
    public string? ProxyIconUrl { get; set; }
}
