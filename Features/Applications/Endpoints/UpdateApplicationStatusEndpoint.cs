using AuthNetExample.Features.Applications.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AuthNetExample.Features.Applications.Endpoints;

public static class UpdateApplicationStatusEndpoint
{
    public static void AddUpdateApplicationStatusEndpoint(this WebApplication app)
    {
        app.MapPut("/api/applications/{id}/status", async (
            int id,
            UpdateApplicationStatusRequest request,
            [FromServices] ApplicationService applicationService) =>
        {
            if (!Enum.TryParse<ApplicationStatus>(request.Status, out var status))
            {
                return Results.BadRequest(new { message = "Invalid application status" });
            }

            var updatedApplication = await applicationService.UpdateApplicationStatusAsync(id, status);

            if (updatedApplication == null)
            {
                return Results.NotFound(new { message = "Application not found" });
            }

            return Results.Ok(updatedApplication);
        })
        .RequireAuthorization();
    }
}
