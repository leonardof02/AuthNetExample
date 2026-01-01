using Jobs.Backend.Features.Shared.Interfaces;

namespace Jobs.Backend.Features.Auth.Errors;

public record RefreshTokenError : IApplicationError
{
    public string Code => "Auth.RefreshTokenError";
    public required string Message { get; init; } = "An unknown error occurred during refresh token processing.";
}