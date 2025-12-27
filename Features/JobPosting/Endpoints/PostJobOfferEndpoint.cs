using System.Security.Claims;
using Features.JobPosting.Models.PostJobOfferRequest;
using Features.JobPosting.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Features.JobPosting.Endpoints;

public static class PostJobOfferEndpoint
{

    public static void AddPostJobOfferEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/joboffers", async (PostJobOfferRequest request, JobPostingService jobPostingService, ClaimsPrincipal claims) =>
        {
            var employerId = claims?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId)) return Results.Unauthorized();
            var jobOffer = await jobPostingService.PostJobOfferAsync(request);
            return Results.Created($"/api/joboffers/{jobOffer.Id}", jobOffer);
        })
        .AddEndpointFilter<ValidationFilter<PostJobOfferRequest>>()
        .RequireAuthorization(
            new AuthorizeAttribute
            {
                Roles = AppRoles.Recruiter,
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            }
        );
    }
}