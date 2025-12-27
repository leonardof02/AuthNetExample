using AuthNetExample.Features.Applications.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthNetExample.Features.Applications.Endpoints;

public static class UpdateApplicationStatusEndpoint
{
    public static void AddUpdateApplicationStatusEndpoint(this WebApplication app)
    {
        app.MapPut("/joboffers/{jobOfferId}/applications/{id}/status", async (
            int jobOfferId,
            int id,
            UpdateApplicationStatusRequest request,
            [FromServices] ApplicationService applicationService) =>
        {
            if (!Enum.TryParse<ApplicationStatus>(request.Status, out var status))
            {
                return Results.BadRequest(new { message = "Invalid application status" });
            }

            var updatedApplication = await applicationService.UpdateApplicationStatusAsync(id, jobOfferId, status);

            if (updatedApplication == null)
            {
                return Results.NotFound(new { message = "Application not found" });
            }

            return Results.Ok(updatedApplication);
        })
        .RequireAuthorization(
            new AuthorizeAttribute
            {
                Roles = AppRoles.Recruiter,
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
            }
        );
    }
}
