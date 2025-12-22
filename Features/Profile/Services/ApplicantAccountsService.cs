using System.Security.Claims;
using AuthNetExample.Features.Profile.Endpoints;
using Microsoft.EntityFrameworkCore;

public class ApplicantAccountService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicantAccountService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApplicantProfile> CreateApplicantProfileAsync(ApplicantProfileRequest request)
    {

        var userId = (_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new Exception("User is not authenticated.");

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
        await _dbContext.SaveChangesAsync();

        return profile;
    }

    public async Task<ApplicantProfile?> GetApplicantProfileByUserIdAsync(string userId)
    {
        return await _dbContext.ApplicantProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<ApplicantProfile?> UpdateApplicantProfileAsync(ApplicantProfileRequest request, int profileId)
    {
        var profile = await _dbContext.ApplicantProfiles.FindAsync(profileId);

        if (profile == null)
        {
            return null;
        }

        profile.FullName = request.FullName;
        profile.ProfileTitle = request.ProfileTitle;
        profile.Bio = request.Bio;
        profile.CvUrl = request.CvUrl;
        profile.WebsiteUrl = request.WebsiteUrl;
        profile.ContactPhone = request.ContactPhone;
        profile.ContactEmail = request.ContactEmail;

        await _dbContext.SaveChangesAsync();

        return profile;
    }
}