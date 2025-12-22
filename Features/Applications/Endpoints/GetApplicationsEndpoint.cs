using Microsoft.AspNetCore.Mvc;

namespace AuthNetExample.Features.Applications.Endpoints;

public static class GetApplicationsEndpoint
{
    public static void AddGetApplicationsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/applications", async (
            [FromServices] ApplicationService applicationService,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        ) =>
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return Results.BadRequest(new { message = "Page number and page size must be greater than 0" });
            }

            var applications = await applicationService.GetApplicationsAsync(pageNumber, pageSize);
            return Results.Ok(applications);
        })
        .RequireAuthorization()
        .WithName("GetApplications")
        .WithTags("Applications")
        .WithOpenApi();
    }
}
