namespace TrfrtSbmt.Api.Utils.DiscordWebhooks;
using System.Text.Json.Serialization;

public class EmbedAuthor
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    /// <summary>
    /// Author name
    /// </summary>
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("url")]
    /// <summary>
    /// Author url
    /// </summary>
    public string? Url { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon_url")]
    /// <summary>
    /// Author icon
    /// </summary>
    public string? IconUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("proxy_icon_url")]
    /// <summary>
    /// Author icon proxy
    /// </summary>
    public string? ProxyIconUrl { get; set; }
}
