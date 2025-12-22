using Features.JobPosting.Models.UpdateJobOfferRequestValidator;
using Features.JobPosting.Services;
using Microsoft.AspNetCore.Mvc;

namespace Features.JobPosting.Endpoints;

public static class UpdateJobOfferEndpoint
{
    public static void AddUpdateJobOfferEndpoint(this WebApplication app)
    {
        app.MapPut("/api/joboffers/{id}", async (int id, UpdateJobOfferRequest request, [FromServices] JobPostingService jobPostingService) =>
        {
            var updatedJobOffer = await jobPostingService.UpdateJobOfferAsync(id, request);
            if (updatedJobOffer == null) return Results.NotFound();
            return TypedResults.Ok(updatedJobOffer);
        })
        .AddEndpointFilter<ValidationFilter<UpdateJobOfferRequest>>()
        .RequireAuthorization();
    }
}