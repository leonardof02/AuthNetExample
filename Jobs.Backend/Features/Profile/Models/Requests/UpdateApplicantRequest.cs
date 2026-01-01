namespace Features.Profile.Models.Requests;

public record UpdateApplicantRequest
{
    public string? FullName { get; init; }
    public string? ProfileTitle { get; init; }
    public string? Bio { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
    public string? WebsiteUrl { get; init; }
}