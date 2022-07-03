namespace TrfrtSbmt.Api.Utils.DiscordWebhooks;
using System.Text.Json.Serialization;

public class EmbedMedia
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("url")]
    /// <summary>
    /// Media url
    /// </summary>
    public string? Url { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("proxy_url")]
    /// <summary>
    /// Media proxy url
    /// </summary>
    public string? ProxyUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("height")]
    /// <summary>
    /// Media height
    /// </summary>
    public int? Height { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("width")]
    /// <summary>
    /// Media width
    /// </summary>
    public int? Width { get; set; }
}
