using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthNetExample.Features.Auth.Models.Constants;

public static class AppClaims
{
    public const string Role = ClaimTypes.Role;
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string UserId = JwtRegisteredClaimNames.Sub;
    public const string UserName = JwtRegisteredClaimNames.UniqueName;
    public const string IsOnboardingCompleted = "is_onboarding_completed";
}