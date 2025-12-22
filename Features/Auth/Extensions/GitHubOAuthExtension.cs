using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;

public static class GitHubOAuthExtension
{
    public static AuthenticationBuilder AddGitHubProvider(
        this AuthenticationBuilder builder,
        IConfiguration configuration
    )
    {
        builder
            .AddGitHub(options =>
            {
                var githubSection = configuration.GetSection("OAuthSettings:Github");
                var clientId = githubSection["ClientId"];
                var clientSecret = githubSection["ClientSecret"];

                options.ClientId = clientId!;
                options.ClientSecret = clientSecret!;
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.CallbackPath = "/signin-github";
                options.Scope.Add("user:email");

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var role = context.Properties.Items["role"];
                        context.Identity?.AddClaim(new Claim(ClaimTypes.Role, role ?? "user"));
                        await Task.CompletedTask;
                    }
                };
            });

        return builder;
    }
}