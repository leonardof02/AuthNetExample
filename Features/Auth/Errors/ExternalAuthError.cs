using AuthNetExample.Features.Shared.Interfaces;

public record ExternalAuthError : IApplicationError
{
    public string Code => "Auth.ExternalAuthError";
    public required string Message { get ; init; } = "An error occurred during external authentication.";
}