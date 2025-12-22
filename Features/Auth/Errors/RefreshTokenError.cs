using AuthNetExample.Features.Shared.Interfaces;

namespace AuthNetExample.Features.Auth.Errors;

public record RefreshTokenError : IApplicationError
{
    public string Code => "Auth.RefreshTokenError";
    public required string Message { get; init; } = "An unknown error occurred during refresh token processing.";
}