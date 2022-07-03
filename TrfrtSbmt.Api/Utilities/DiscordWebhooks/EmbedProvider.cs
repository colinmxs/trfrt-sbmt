namespace TrfrtSbmt.Api.Utils.DiscordWebhooks;
using System.Text.Json.Serialization;

public class EmbedProvider
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    /// <summary>
    /// Provider name
    /// </summary>
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("url")]
    /// <summary>
    /// Provider url
    /// </summary>
    public string? Url { get; set; }
}
