using System.Security.Claims;
using AspNet.Security.OAuth.GitHub;
using AuthNetExample.Features.Auth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Features.Auth.Endpoints;

public static class SignInWithGithub
{

    public static void AddSignInWithGithubEndpoint(this WebApplication app)
    {
        app.MapGet("/auth/github/callback", async (
            ClaimsPrincipal user,
            HttpContext context,
            AuthService authService
        ) =>
        {
            var email = user.FindFirstValue(ClaimTypes.Email);
            var nameIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (email == null || nameIdentifier == null)
                return Results.BadRequest("Email or NameIdentifier not provided by GitHub.");

            var result = await authService.SignInWithGithubAsync(email, nameIdentifier);
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

            return result.IsSuccess
                ? Results.Redirect("/main")
                : Results.BadRequest(result.Error);
        })
        .RequireAuthorization(
            new AuthorizeAttribute
            {
                AuthenticationSchemes = GitHubAuthenticationDefaults.AuthenticationScheme
            }
        );
    }
}
