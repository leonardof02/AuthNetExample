using Jobs.Backend.Features.Auth.Services;
using Features.Auth.Models.Requests.RegisterRequest;

public static class MapRegisterUser
{
    public static void MapRegisterUserEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/signup", async (SignUpRequest request, AuthService authService, CancellationToken cancellationToken) =>
        {
            var result = await authService.RegisterUserAsync(request);
            return Results.Ok(result);
        })
        .AddEndpointFilter<ValidationFilter<SignUpRequest>>();
    } 
}