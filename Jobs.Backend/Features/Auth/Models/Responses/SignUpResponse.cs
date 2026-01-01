public record SignUpResponse
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public required string Role { get; init; }
    public required string Email { get; init; }
    public required DateTime RegisteredAt { get; init; }
    public required string Token { get; init; }
    public required string RefreshToken { get; init; }
}