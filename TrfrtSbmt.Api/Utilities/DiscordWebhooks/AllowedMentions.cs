namespace TrfrtSbmt.Api.Utils.DiscordWebhooks;
using System.Text.Json.Serialization;

public class AllowedMentions
{
    [JsonPropertyName("parse")]
    /// <summary>
    /// List of allowd mention types to parse from the content
    /// </summary>
    public List<string> Parse { get; set; } = new List<string>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("roles")]
    /// <summary>
    /// List of role_ids to mention
    /// </summary>
    public List<ulong>? Roles { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("users")]
    /// <summary>
    /// List of user_ids to mention
    /// </summary>
    public List<ulong>? Users { get; set; }
}