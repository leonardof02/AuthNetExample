using AuthNetExample.Features.Auth.Models;
using AuthNetExample.Features.Auth.Services;

public static class RefreshTokenEndpoint
{
    public static void AddRefreshTokenEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/refresh", async (RefreshTokenRequest request, AuthService authService) =>
        {
            var result = await authService.RefreshTokenAsync(request.RefreshToken);
            return Results.Ok(result);
        })
        .AddEndpointFilter<ValidationFilter<RefreshTokenRequest>>()
        .AllowAnonymous();
    }
}