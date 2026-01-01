using System.Security.Claims;
using Jobs.Backend.Features.Notifications.Services;
using Microsoft.EntityFrameworkCore;

public class ApplicationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TelegramBotService _telegramBotService;

    public ApplicationService(
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        TelegramBotService telegramBotService
    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _telegramBotService = telegramBotService;
    }

    public async Task<JobApplication> SubmitApplicationAsync(int jobOfferId)
    {

        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User is not authenticated.");

        var jobOffer = await _dbContext.JobOffers.FindAsync(jobOfferId)
            ?? throw new Exception("Job offer not found.");

        var existingApplication = await _dbContext.JobApplications
            .FirstOrDefaultAsync(ja => ja.JobOfferId == jobOfferId && ja.ApplicantId == userId);

        if (existingApplication != null)
            throw new Exception("You have already applied for this job offer.");

        var application = new JobApplication
        {
            JobOfferId = jobOfferId,
            ApplicantId = userId,
            AppliedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Pending
        };

        _dbContext.JobApplications.Add(application);
        await _dbContext.SaveChangesAsync();

        var recruiterId = jobOffer.EmployerId;
        await _telegramBotService.SendNotificationToUserAsync(recruiterId, $"New application received for your job offer '{jobOffer.Title}'.");

        return application;
    }

    public async Task<PaginatedResponse<JobApplication>> GetApplicationsByJobOfferIdAsync(int jobOfferId, int pageNumber, int pageSize)
    {

        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User is not authenticated.");

        var jobOffer = await _dbContext.JobOffers.FindAsync(jobOfferId)
            ?? throw new Exception("Job offer not found.");

        if (jobOffer.EmployerId != userId)
            throw new Exception("Unauthorized to view applications for this job offer.");

        var applications = await _dbContext.JobApplications
            .Where(ja => ja.JobOfferId == jobOfferId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<JobApplication>
        (
            applications,
            await _dbContext.JobApplications.CountAsync(ja => ja.JobOfferId == jobOfferId),
            pageSize,
            pageNumber,
            (int)Math.Ceiling((double)await _dbContext.JobApplications.CountAsync(ja => ja.JobOfferId == jobOfferId) / pageSize)
        );
    }

    public async Task<PaginatedResponse<JobApplication>> GetAllApplicationsByApplicantId(string applicantId, int pageNumber, int pageSize)
    {

        var applications = await _dbContext.JobApplications
            .Where(ja => ja.ApplicantId == applicantId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new PaginatedResponse<JobApplication>
        (
            applications,
            await _dbContext.JobApplications.CountAsync(ja => ja.ApplicantId == applicantId),
            pageSize,
            pageNumber,
            (int)Math.Ceiling((double)await _dbContext.JobApplications.CountAsync(ja => ja.ApplicantId == applicantId) / pageSize)
        );

        return response;
    }

    public async Task<JobApplication?> UpdateApplicationStatusAsync(int applicationId, int jobOfferId, ApplicationStatus newStatus)
    {

        var recruiterId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User is not authenticated.");

        var jobOffer = await _dbContext.JobOffers.FindAsync(jobOfferId)
            ?? throw new Exception("Job offer not found.");

        if (jobOffer.EmployerId != recruiterId)
        {
            throw new Exception("Unauthorized to update application status for this job offer.");
        }

        var application = await _dbContext.JobApplications.FindAsync(applicationId);
        if (application == null) throw new Exception("Application not found.");

        application.Status = newStatus;
        await _dbContext.SaveChangesAsync();

        var applicantId = application.ApplicantId;
        await _telegramBotService.SendNotificationToUserAsync(applicantId, $"Your application for the job offer '{jobOffer.Title}' has been updated to '{newStatus}'.");

        return application;
    }

    public async Task<int> DeleteApplicationAsync(int applicationId)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User is not authenticated.");

        var application = await _dbContext.JobApplications.FindAsync(applicationId)
            ?? throw new Exception("Application not found.");

        if (userId != application.ApplicantId)
            throw new Exception("Unauthorized to delete this application.");

        if (application == null) throw new Exception("Application not found.");

        _dbContext.JobApplications.Remove(application);
        await _dbContext.SaveChangesAsync();

        return applicationId;
    }

    public async Task<PaginatedResponse<JobApplication>> GetApplicationsAsync(int pageNumber, int pageSize)
    {

        var applicationsQuery = _dbContext.JobApplications.AsQueryable();

        var applications = await applicationsQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await applicationsQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginatedResponse<JobApplication>(applications, totalCount, pageSize, pageNumber, totalPages);
    }
}