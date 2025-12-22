public class JobApplication
{
    public int Id { get; set; }
    public int JobOfferId { get; set; }
    public int ApplicantId { get; set; }
    public DateTime AppliedAt { get; set; }
    public ApplicationStatus Status { get; set; }
}