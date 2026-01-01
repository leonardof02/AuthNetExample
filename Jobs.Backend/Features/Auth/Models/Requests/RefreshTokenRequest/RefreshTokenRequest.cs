namespace Jobs.Backend.Features.Auth.Models;

public record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}