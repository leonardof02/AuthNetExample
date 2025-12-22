using AuthNetExample.Features.Shared.Interfaces;

namespace AuthNetExample.Features.Auth.Errors;

public record LoginError : IApplicationError
{
    public string Code => "Auth.LoginError";
    public required string Message { get; init; } = "An unknown error occurred during login.";
}