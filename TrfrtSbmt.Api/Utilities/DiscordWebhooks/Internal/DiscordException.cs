namespace TrfrtSbmt.Api.Utils.DiscordWebhooks.Internal;

internal class DiscordException : Exception
{
    internal DiscordException()
    {
    }

    internal DiscordException(string message)
        : base(message)
    {
    }

    internal DiscordException(string message, Exception inner)
        : base(message, inner)
    {
    }
}