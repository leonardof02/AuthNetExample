namespace AuthNetExample.Features.Applications.Models.Requests;

public record SubmitApplicationRequest
{
    public required int JobOfferId { get; init; }
}
