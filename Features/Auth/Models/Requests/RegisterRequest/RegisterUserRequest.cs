namespace Features.Auth.Models.Requests.RegisterRequest;

public record RegisterUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}