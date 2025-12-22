using System.Security.Claims;
using Features.JobPosting.Models.PostJobOfferRequest;
using Features.JobPosting.Services;

namespace Features.JobPosting.Endpoints;

public static class PostJobOfferEndpoint
{

    public static void AddPostJobOfferEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/joboffers", async (PostJobOfferRequest request, JobPostingService jobPostingService, ClaimsPrincipal claims) =>
        {
            var employerId = claims?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId)) return Results.Unauthorized();
            var jobOffer = await jobPostingService.PostJobOfferAsync(request, employerId);
            return Results.Created($"/api/joboffers/{jobOffer.Id}", jobOffer);
        })
        .AddEndpointFilter<ValidationFilter<PostJobOfferRequest>>()
        .RequireAuthorization();
    }
}