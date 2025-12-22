using AuthNetExample.Features.Auth.Models;
using AuthNetExample.Features.Auth.Services;

public static class RefreshTokenEndpoint
{
    public static void AddRefreshTokenEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/refresh", async (RefreshTokenRequest request, AuthService authService) =>
        {
            var result = await authService.RefreshTokenAsync(request.RefreshToken);
            if (!result.IsSuccess) return Results.BadRequest(result.Error);
            return TypedResults.Ok(result.Value);
        })
        .AddEndpointFilter<ValidationFilter<RefreshTokenRequest>>()
        .AllowAnonymous();
    }
}