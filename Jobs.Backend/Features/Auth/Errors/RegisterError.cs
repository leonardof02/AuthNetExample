using Jobs.Backend.Features.Shared.Interfaces;

namespace Jobs.Backend.Features.Auth.Errors;

public class RegisterError : IApplicationError
{
    public string Code => "Auth.RegisterError";
    public string Message { get; init; } = "An unknown error occurred during user registration.";
}