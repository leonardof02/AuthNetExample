
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace AuthNetExample.Features.Profile.Services;

public record RecruiterProfileRequest(
    string FullName,
    string ProfileTitle,
    string Bio,
    string ContactPhone,
    string ContactEmail
);

public class RecruiterAccountsService
{
    
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecruiterAccountsService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RecruiterProfile> CreateRecruiterAccountAsync(RecruiterProfileRequest profile)
    {
        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

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
        await _dbContext.SaveChangesAsync();
        return recruiterProfile;
    }

    public async Task<RecruiterProfile?> GetRecruiterProfileByUserIdAsync(string userId)
    {
        return await _dbContext.RecruiterProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<RecruiterProfile?> UpdateRecruiterProfileAsync(RecruiterProfileRequest request, int profileId)
    {
        var profile = await _dbContext.RecruiterProfiles.FindAsync(profileId);

        if (profile == null)
        {
            return null;
        }

        profile.FullName = request.FullName;
        profile.ProfileTitle = request.ProfileTitle;
        profile.Bio = request.Bio;
        profile.ContactPhone = request.ContactPhone;
        profile.ContactEmail = request.ContactEmail;

        await _dbContext.SaveChangesAsync();

        return profile;
    }

    public async Task<bool> AddCompanyToRecruiter(int companyId)
    {
        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

        var company = await _dbContext.Companies.FindAsync(companyId) ?? throw new Exception("Company not found.");;
        var recruiterProfile = await _dbContext.RecruiterProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId)
            ?? throw new Exception("Recruiter profile not found.");


        company.RecruiterId = userId;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }
}