public record JobApplicationResponse
{
    public int Id { get; init; }
    public int JobOfferId { get; init; }
    public int ApplicantId { get; init; }
    public DateTime AppliedAt { get; init; }
    public string Status { get; init; }
}
