using AuthNetExample.Features.Applications.Models.Requests;
using Microsoft.AspNetCore.Authorization;

namespace AuthNetExample.Features.Applications.Endpoints;

public static class SubmitApplicationEndpoint
{
    public static void AddSubmitApplicationEndpoint(this WebApplication app)
    {
        app.MapPost("/api/applications", async (
            SubmitApplicationRequest request,
            ApplicationService applicationService) =>
        {
            var application = await applicationService.SubmitApplicationAsync(request.JobOfferId);
            
            return Results.Created($"/api/applications/{application.Id}", new
            {
                application.Id,
                application.JobOfferId,
                application.ApplicantId,
                application.AppliedAt,
                Status = application.Status.ToString()
            });
        })
        .RequireAuthorization(
            new AuthorizeAttribute
            {
                Roles = "applicant"
            }
        )
        .WithName("SubmitApplication")
        .WithTags("Applications")
        .WithOpenApi();
    }
}
