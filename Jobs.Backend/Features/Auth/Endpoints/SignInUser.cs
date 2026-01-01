using Jobs.Backend.Features.Auth.Models;
using Jobs.Backend.Features.Auth.Services;

public static class LoginUser
{ 
    public static void AddLoginUserEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/signin", async (LoginRequest request, AuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            return Results.Ok(result);
        })
        .AddEndpointFilter<ValidationFilter<LoginRequest>>()
        .AllowAnonymous();
    }
}