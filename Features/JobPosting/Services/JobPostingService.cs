using Features.JobPosting.Models;
using Features.JobPosting.Models.GetJobOffersParams;
using Features.JobPosting.Models.PostJobOfferRequest;
using Microsoft.EntityFrameworkCore;

namespace Features.JobPosting.Services;

public class JobPostingService
{
    private readonly ApplicationDbContext _dbContext;

    public JobPostingService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<JobOffer> PostJobOfferAsync(PostJobOfferRequest request, string employerId)
    {
        var jobOffer = new JobOffer(
            request.Title,
            request.Description,
            request.Location,
            request.MinSalary,
            request.MaxSalary,
            employerId
        );

        _dbContext.JobOffers.Add(jobOffer);
        await _dbContext.SaveChangesAsync();

        return jobOffer;
    }

    public async Task<JobOffer?> GetJobOfferByIdAsync(int id)
    {
        return await _dbContext.JobOffers.FindAsync(id);
    }

    public async Task<PaginatedResponse<JobOffer>> GetJobOffersAsync(GetJobOffersParams queryParams)
    {

        var jobOffersQuery = _dbContext.JobOffers.AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.EmployerIdFilter))
        {
            jobOffersQuery = jobOffersQuery.Where(jo => jo.EmployerId == queryParams.EmployerIdFilter);
        }

        if (!string.IsNullOrEmpty(queryParams.LocationFilter))
        {
            jobOffersQuery = jobOffersQuery.Where(jo => jo.Location.Contains(queryParams.LocationFilter));
        }

        if (queryParams.MinSalaryFilter.HasValue)
        {
            jobOffersQuery = jobOffersQuery.Where(jo => jo.MinSalary >= queryParams.MinSalaryFilter.Value);
        }

        if (queryParams.MaxSalaryFilter.HasValue)
        {
            jobOffersQuery = jobOffersQuery.Where(jo => jo.MaxSalary <= queryParams.MaxSalaryFilter.Value);
        }

        var jobOffers = await jobOffersQuery
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return CreatePaginatedResponse(jobOffers, queryParams.PageNumber, queryParams.PageSize, await jobOffersQuery.CountAsync());
    }

    private PaginatedResponse<JobOffer> CreatePaginatedResponse(List<JobOffer> items, int pageNumber, int pageSize, int totalCount)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PaginatedResponse<JobOffer>(items, totalCount, pageSize, pageNumber, totalPages);
    }

    public async Task<JobOffer?> UpdateJobOfferAsync(int id, UpdateJobOfferRequest request)
    {
        var jobOffer = await _dbContext.JobOffers.FindAsync(id);
        
        if (jobOffer == null)
        {
            return null;
        }

        jobOffer.Title = request.Title;
        jobOffer.Description = request.Description;
        jobOffer.Location = request.Location;
        jobOffer.MinSalary = request.MinSalary;
        jobOffer.MaxSalary = request.MaxSalary;

        await _dbContext.SaveChangesAsync();
        return jobOffer;
    }

    public async Task<int?> DeleteJobOfferAsync(int id)
    {
        var jobOffer = await _dbContext.JobOffers.FindAsync(id);
        
        if (jobOffer == null)
        {
            return null;
        }

        _dbContext.JobOffers.Remove(jobOffer);
        await _dbContext.SaveChangesAsync();
        return jobOffer.Id;
    }
}