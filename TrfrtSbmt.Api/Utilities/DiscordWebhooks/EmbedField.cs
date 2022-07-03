namespace TrfrtSbmt.Api.Utils.DiscordWebhooks;
using System.Text.Json.Serialization;

public class EmbedField
{
    [JsonPropertyName("name")]
    /// <summary>
    /// Field name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    /// <summary>
    /// Field value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("inline")]
    /// <summary>
    /// Field align
    /// </summary>
    public bool? InLine { get; set; }
}
