using Jobs.Backend.Features.Shared.Interfaces;

public record ExternalSignInError : IApplicationError
{
    public string Code => "Auth.ExternalSignInError";
    public required string Message { get ; init; } = "An error occurred during external sign-in.";
}