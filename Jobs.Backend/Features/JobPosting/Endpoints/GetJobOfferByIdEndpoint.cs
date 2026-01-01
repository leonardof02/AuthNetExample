using Features.JobPosting.Services;
using Microsoft.AspNetCore.Mvc;

namespace Features.JobPosting.Endpoints;

public static class GetJobOfferById
{
    public static void AddGetJobOfferByIdEndpoint(this WebApplication app)
    {
        app.MapGet("/joboffers/{id}", async (int id, [FromServices] JobPostingService jobPostingService) =>
        {
            var jobOffer = await jobPostingService.GetJobOfferByIdAsync(id);
            if (jobOffer == null) return Results.NotFound();
            return TypedResults.Ok(jobOffer);
        })
        .RequireAuthorization();
    }
}