using Jobs.Backend.Features.Auth.Models.Params;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Features.Auth.Endpoints;

public static class OpenGithubLoginPage
{
    public static void AddOpenGithubLoginPageEndpoint(this WebApplication app)
    {
        app.MapGet("/auth/github/{role}", async (
            HttpContext context,
            [FromRoute] UserRole role
        ) =>
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/auth/github/callback"
            };

            properties.Items["role"] = role.Value;
            await context.ChallengeAsync("GitHub", properties);
            return;
        })
        .AllowAnonymous();
    }
}
