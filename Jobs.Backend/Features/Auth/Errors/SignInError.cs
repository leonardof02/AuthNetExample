using Jobs.Backend.Features.Shared.Interfaces;

namespace Jobs.Backend.Features.Auth.Errors;

public record SignInError : IApplicationError
{
    public string Code => "Auth.SignInError";
    public required string Message { get; init; } = "An unknown error occurred during sign-in.";
}