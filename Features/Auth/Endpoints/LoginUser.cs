using AuthNetExample.Features.Auth.Models;
using AuthNetExample.Features.Auth.Services;

public static class LoginUser
{
    public static void AddLoginUserEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/login", async (LoginRequest request, AuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            if (!result.IsSuccess) return Results.Unauthorized();
            return TypedResults.Ok(result.Value);
        })
        .AddEndpointFilter<ValidationFilter<LoginRequest>>()
        .AllowAnonymous();
    }
}