namespace AuthNetExample.Features.Auth.Models;

public record AuthResponse
{
    public required string Token { get; init; }
    public required string Email { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required string RefreshToken { get; init; }
}
