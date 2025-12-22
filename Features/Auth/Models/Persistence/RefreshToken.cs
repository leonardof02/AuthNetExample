using System.Security.Cryptography;

namespace AuthNetExample.Features.Auth.Models;

public class RefreshToken
{

    public string Token { get; init; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddDays(7);
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public string UserId { get; init; }

    public RefreshToken(string userId)
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        UserId = userId;
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }


    public bool IsActive => RevokedAt == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}