using System.ComponentModel.DataAnnotations;

namespace Jobs.Backend.Features.Auth.Models;

public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}