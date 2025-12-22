using System.Security.Claims;
using AuthNetExample.Features.Applications.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AuthNetExample.Features.Applications.Endpoints;

public static class SubmitApplicationEndpoint
{
    public static void AddSubmitApplicationEndpoint(this WebApplication app)
    {
        app.MapPost("/api/applications", async (
            SubmitApplicationRequest request,
            ApplicationService applicationService,
            ClaimsPrincipal claims) =>
        {
            var applicantIdString = claims?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(applicantIdString))
            {
                return Results.Unauthorized();
            }

            var application = await applicationService.SubmitApplicationAsync(request.JobOfferId, applicantIdString);
            
            return Results.Created($"/api/applications/{application.Id}", new
            {
                application.Id,
                application.JobOfferId,
                application.ApplicantId,
                application.AppliedAt,
                Status = application.Status.ToString()
            });
        })
        .RequireAuthorization()
        .WithName("SubmitApplication")
        .WithTags("Applications")
        .WithOpenApi();
    }
}
