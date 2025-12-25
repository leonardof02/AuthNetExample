using AuthNetExample.Features.Auth.Services;
using Features.Auth.Models.Requests.RegisterRequest;

public static class MapRegisterUser
{
    public static void MapRegisterUserEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/register", async (RegisterUserRequest request, AuthService authService, CancellationToken cancellationToken) =>
        {
            var result = await authService.RegisterUserAsync(request);
            return Results.Ok(result);
        })
        .AddEndpointFilter<ValidationFilter<RegisterUserRequest>>();
    }
}