public class UserProfile 
{
    public int Id { get; set; }
    public required string FullName { get; set; } = string.Empty;
    public string ProfileTitle { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? ContactPhone { get; set; } = string.Empty;
    public string? ContactEmail { get; set; } = string.Empty;

    public required string UserId { get; set; } = string.Empty;
}