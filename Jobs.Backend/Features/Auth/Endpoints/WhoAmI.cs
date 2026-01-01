using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

public static class WhoAmI
{
    const string Route = "/auth/whoami";

    public static void AddWhoAmIEndpoint(this WebApplication app)
    {
        app.MapGet(Route, async (ClaimsPrincipal claims, UserManager<Features.Shared.Persistence.ApplicationUser> userManager) =>
        {
            var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = claims.FindFirstValue(ClaimTypes.Name);
            var email = claims.FindFirstValue(ClaimTypes.Email);

            return TypedResults.Ok(new
            {
                UserId = userId,
                Username = username,
                Email = email
            });
        })
        .RequireAuthorization();
    }
}