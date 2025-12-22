namespace AuthNetExample.Features.Auth.Models;

public record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}