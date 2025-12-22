namespace Features.Auth.Models.Requests.RegisterRequest;

public record RegisterUserRequest
{
    public string? Email { get; init; }
    public string? Username { get; init; }

    public string? Password { get; init; }
    public string? ConfirmPassword { get; init; }
    public required string Role { get; init; }
}