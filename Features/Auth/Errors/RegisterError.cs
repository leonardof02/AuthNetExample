using AuthNetExample.Features.Shared.Interfaces;

namespace AuthNetExample.Features.Auth.Errors;

public class RegisterError : IApplicationError
{
    public string Code => "Auth.RegisterError";
    public string Message { get; init; } = "An unknown error occurred during user registration.";
}