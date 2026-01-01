using Jobs.Backend.Features.Shared.Interfaces;

namespace Jobs.Backend.Features.Auth.Errors;

public record SignUpError : IApplicationError
{
    public string Code => "Auth.SignUpError";
    public required string Message { get; init; } = "An unknown error occurred during sign-up.";
}