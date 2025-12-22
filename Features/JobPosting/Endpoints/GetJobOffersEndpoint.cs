using Features.JobPosting.Models.GetJobOffersParams;
using Features.JobPosting.Services;

namespace Features.JobPosting.Endpoints;

public static class GetJobOffersEndpoint
{
    public static void AddGetJobOffersEndpoint(this WebApplication app)
    {
        app.MapGet("/api/joboffers", async ([AsParameters] GetJobOffersParams queryParams, JobPostingService jobPostingService) =>
        {
            var jobOffers = await jobPostingService.GetJobOffersAsync(queryParams);
            return TypedResults.Ok(jobOffers);
        })
        .AddEndpointFilter<ValidationFilter<GetJobOffersParams>>()
        .RequireAuthorization();
    }
}