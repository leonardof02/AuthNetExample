using Features.JobPosting.Services;
using Microsoft.AspNetCore.Mvc;

namespace Features.JobPosting.Endpoints;

public static class DeleteJobOfferEndpoint
{
    public static void AddDeleteJobOfferEndpoint(this WebApplication app)
    {
        app.MapDelete("/joboffers/{id}", async (int id, [FromServices] JobPostingService jobPostingService) =>
        {
            var deletedId = await jobPostingService.DeleteJobOfferAsync(id);
            if (deletedId == null) return Results.NotFound();
            return Results.NoContent();
        })
        .RequireAuthorization();
    }
}