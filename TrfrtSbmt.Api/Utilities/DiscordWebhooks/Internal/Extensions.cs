namespace TrfrtSbmt.Api.Utils.DiscordWebhooks.Internal;

using System.Drawing;

internal static class Extensions
{
    internal static int ToHex(this Color color)
    {
        string HS =
            color.R.ToString("X2") +
            color.G.ToString("X2") +
            color.B.ToString("X2");

        return int.Parse(HS, System.Globalization.NumberStyles.HexNumber);
    }

    internal static Color ToColor(this int hex) => ColorTranslator.FromHtml(hex.ToString("X6"));
}