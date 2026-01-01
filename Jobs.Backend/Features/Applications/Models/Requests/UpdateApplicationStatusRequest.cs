namespace Jobs.Backend.Features.Applications.Models.Requests;

public record UpdateApplicationStatusRequest
{
    public required string Status { get; init; }
}
