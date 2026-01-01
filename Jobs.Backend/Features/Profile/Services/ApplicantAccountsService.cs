using System.Security.Claims;
using Features.Profile.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Features.Profile.Services;

public class ApplicantAccountService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicantAccountService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApplicantProfile> CreateApplicantProfileAsync(CreateApplicantProfileRequest request)
    {

        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

        var user = await _dbContext.Users.FindAsync(userId) ?? throw new Exception("User not found.");
        if (user.IsOnboardingCompleted) throw new Exception("User has already completed onboarding and have a profile.");

        var profile = new ApplicantProfile
        {
            FullName = request.FullName,
            ProfileTitle = request.ProfileTitle,
            Bio = request.Bio,
            CvUrl = request.CvUrl,
            WebsiteUrl = request.WebsiteUrl,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail,
            UserId = userId
        };

        _dbContext.ApplicantProfiles.Add(profile);
        user.IsOnboardingCompleted = true;

        await _dbContext.SaveChangesAsync();
        return profile;
    }

    public async Task<ApplicantProfile?> GetApplicantProfileByUserIdAsync(string userId)
    {
        return await _dbContext.ApplicantProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<ApplicantProfile> UpdateApplicantProfileAsync(UpdateApplicantRequest request, int profileId)
    {

        var profile = await _dbContext.ApplicantProfiles.FindAsync(profileId) ??
            throw new Exception("Profile not found.");

        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

        if (profile.UserId != userId)
            throw new Exception("User is not authorized to update this profile.");

        if (!string.IsNullOrEmpty(request.FullName)) profile.FullName = request.FullName;
        if (!string.IsNullOrEmpty(request.ProfileTitle)) profile.ProfileTitle = request.ProfileTitle;
        if (!string.IsNullOrEmpty(request.Bio)) profile.Bio = request.Bio;
        if (!string.IsNullOrEmpty(request.WebsiteUrl)) profile.WebsiteUrl = request.WebsiteUrl;
        if (!string.IsNullOrEmpty(request.ContactPhone)) profile.ContactPhone = request.ContactPhone;
        if (!string.IsNullOrEmpty(request.ContactEmail)) profile.ContactEmail = request.ContactEmail;

        await _dbContext.SaveChangesAsync();

        return profile;
    }
}