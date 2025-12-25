using System.Security.Claims;
using Features.Profile.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace AuthNetExample.Features.Profile.Services;

public class RecruiterAccountsService
{

    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecruiterAccountsService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RecruiterProfile> CreateRecruiterAccountAsync(CreateRecruiterProfileRequest profile)
    {
        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

        var user = await _dbContext.Users.FindAsync(userId) ?? throw new Exception("User not found.");
        if (user.IsOnboardingCompleted) throw new Exception("User has already completed onboarding and have a profile.");

        var recruiterProfile = new RecruiterProfile
        {
            FullName = profile.FullName,
            ProfileTitle = profile.ProfileTitle,
            Bio = profile.Bio,
            ContactPhone = profile.ContactPhone,
            ContactEmail = profile.ContactEmail,
            UserId = userId
        };

        _dbContext.RecruiterProfiles.Add(recruiterProfile);
        user.IsOnboardingCompleted = true;

        await _dbContext.SaveChangesAsync();
        return recruiterProfile;
    }

    public async Task<RecruiterProfile?> GetRecruiterProfileByUserIdAsync(string userId)
    {
        return await _dbContext.RecruiterProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<RecruiterProfile> UpdateRecruiterProfileAsync(UpdateRecruiterRequest request, int profileId)
    {
        var profile = await _dbContext.RecruiterProfiles.FindAsync(profileId)
            ?? throw new Exception("Profile not found.");

        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

        if (profile.UserId != userId)
            throw new Exception("User is not authorized to update this profile.");


        if (!string.IsNullOrWhiteSpace(request.FullName))
            profile.FullName = request.FullName;
        if (!string.IsNullOrWhiteSpace(request.ProfileTitle))
            profile.ProfileTitle = request.ProfileTitle;
        if (!string.IsNullOrWhiteSpace(request.Bio))
            profile.Bio = request.Bio;
        if (!string.IsNullOrWhiteSpace(request.ContactPhone))
            profile.ContactPhone = request.ContactPhone;
        if (!string.IsNullOrWhiteSpace(request.ContactEmail))
            profile.ContactEmail = request.ContactEmail;

        await _dbContext.SaveChangesAsync();

        return profile;
    }

    public async Task<bool> AddCompanyToRecruiter(int companyId)
    {
        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

        var company = await _dbContext.Companies.FindAsync(companyId) ?? throw new Exception("Company not found."); ;
        var recruiterProfile = await _dbContext.RecruiterProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId)
            ?? throw new Exception("Recruiter profile not found.");


        company.RecruiterId = userId;

        await _dbContext.SaveChangesAsync();
        return true;
    }
}