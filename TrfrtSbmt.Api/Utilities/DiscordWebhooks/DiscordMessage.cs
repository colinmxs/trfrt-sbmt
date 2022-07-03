namespace TrfrtSbmt.Api.Utils.DiscordWebhooks;
using System.Text.Json.Serialization;

public class DiscordMessage
{
    [JsonPropertyName("content")]
    /// <summary>
    /// Message content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("tts")]
    /// <summary>
    /// Read message to everyone on the channel
    /// </summary>
    public bool TTS { get; set; }

    [JsonPropertyName("username")]
    /// <summary>
    /// Webhook profile username to be shown
    /// </summary>
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("avatar_url")]
    /// <summary>
    /// Webhook profile avater to be shown
    /// </summary>
    public string AvatarUrl { get; set; } = string.Empty;

    [JsonPropertyName("embeds")]
    /// <summary>
    /// List of embeds
    /// </summary>
    public List<DiscordEmbed> Embeds { get; set; } = new List<DiscordEmbed>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allowed_mentions")]
    /// <summary>
    /// Allowed mentions for this message
    /// </summary>
    public AllowedMentions AllowedMentions { get; set; } = new AllowedMentions();
}
