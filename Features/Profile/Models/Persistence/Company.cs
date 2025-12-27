public class Company
{
    public int Id { get; set; }
    public required string RecruiterId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; } = null;
    public string? Email { get; set; } = null;
    public string? PhoneNumber { get; set; } = null;
    public string? Website { get; set; } = null;
}