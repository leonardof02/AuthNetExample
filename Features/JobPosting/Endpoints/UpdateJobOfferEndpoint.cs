using Features.JobPosting.Models.UpdateJobOfferRequestValidator;
using Features.JobPosting.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Features.JobPosting.Endpoints;

public static class UpdateJobOfferEndpoint
{
    public static void AddUpdateJobOfferEndpoint(this WebApplication app)
    {
        app.MapPut("/joboffers/{id}", async (int id, UpdateJobOfferRequest request, [FromServices] JobPostingService jobPostingService) =>
        {
            var updatedJobOffer = await jobPostingService.UpdateJobOfferAsync(id, request);
            return TypedResults.Ok(updatedJobOffer);
        })
        .AddEndpointFilter<ValidationFilter<UpdateJobOfferRequest>>()
        .RequireAuthorization(
            new AuthorizeAttribute
            {
                Roles = AppRoles.Recruiter,
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
            }
        );
    }
}