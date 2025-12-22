using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class ApplicationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<JobApplication> SubmitApplicationAsync(int jobOfferId)
    {

        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User is not authenticated.");

        var application = new JobApplication
        {
            JobOfferId = jobOfferId,
            ApplicantId = userId,
            AppliedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Pending
        };

        _dbContext.JobApplications.Add(application);
        await _dbContext.SaveChangesAsync();

        return application;
    }

    public async Task<PaginatedResponse<JobApplication>> GetApplicationsByJobOfferIdAsync(int jobOfferId, int pageNumber, int pageSize)
    {
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

    public async Task<JobApplication?> UpdateApplicationStatusAsync(int id, ApplicationStatus newStatus)
    {
        var application = await _dbContext.JobApplications.FindAsync(id);
        if (application == null) return null;

        application.Status = newStatus;
        await _dbContext.SaveChangesAsync();

        return application;
    }

    public async Task<int?> DeleteApplicationAsync(int id)
    {
        var application = await _dbContext.JobApplications.FindAsync(id);
        if (application == null) return null;

        _dbContext.JobApplications.Remove(application);
        await _dbContext.SaveChangesAsync();

        return id;
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