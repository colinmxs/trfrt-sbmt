namespace TrfrtSbmt.Api.Utilities;

using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Security.Claims;
using TrfrtSbmt.Api.Utils.DiscordWebhooks;

public class DiscordExceptionLogger
{
    private readonly RequestDelegate _next;

    public DiscordExceptionLogger(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IDiscordWebhookClient client)
    {

        // 
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            string? username = context.User.Claims.SingleOrDefault(c => c.Type == "username")?.Value;

            try
            {
                var message = new DiscordMessage
                {
                    Username = $"User: {username ?? "<Unknown>"}",
                    Embeds = new List<DiscordEmbed>
                    {
                        new DiscordEmbed
                        {
                            Title = $"{TrimTo(e.Message, 253)}...",
                            Description = e.InnerException == null ? TrimTo(e.StackTrace, 4096) : null,
                            Fields = e.InnerException != null ? new List<EmbedField>
                            {
                                new EmbedField
                                {
                                    Name = TrimTo(e.InnerException.GetType().Name, 256),
                                    Value = TrimTo(e.InnerException.Message, 1024)
                                }
                            } : null,
                            Color = Color.Red,
                            Footer = e.InnerException != null ? new EmbedFooter
                            {
                                Text = TrimTo(e.InnerException.StackTrace, 4096)
                            } : null
                        }
                    }
                };

                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw new AggregateException(ex, e);
            }
            throw;
        }
    }

    private static string TrimTo(string @string, int length) 
    {
        if (@string.Length > length) @string = @string.Remove(length, @string.Length - length);
        return @string;
    }
}
