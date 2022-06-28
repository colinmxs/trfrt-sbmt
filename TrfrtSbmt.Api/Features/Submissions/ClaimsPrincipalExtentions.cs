namespace TrfrtSbmt.Api.Features.Submissions;
using System.Security.Claims;

public static class ClaimsPrincipalExtentions
{
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Value == "admin");
    }

    public static bool IsVoter(this ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Value == "voter");
    }
}
