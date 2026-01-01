namespace Features.Profile.Models.Requests;

public record CreateApplicantProfileRequest
{
    public string FullName { get; init; } = string.Empty;
    public string ProfileTitle { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string? CvUrl { get; init; } = string.Empty;
    public string? WebsiteUrl { get; init; } = string.Empty;
}