using Microsoft.AspNetCore.Identity;

namespace Features.Shared.Persistence;

public class ApplicationUser : IdentityUser
{
    public bool IsOnboardingCompleted { get; set; } = false;
}