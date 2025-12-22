using Microsoft.AspNetCore.Mvc;

namespace AuthNetExample.Features.Applications.Endpoints;

public static class DeleteApplicationEndpoint
{
    public static void AddDeleteApplicationEndpoint(this WebApplication app)
    {
        app.MapDelete("/api/applications/{id}", async (
            int id,
            [FromServices] ApplicationService applicationService) =>
        {
            var deletedId = await applicationService.DeleteApplicationAsync(id);

            if (deletedId == null)
            {
                return Results.NotFound(new { message = "Application not found" });
            }

            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithName("DeleteApplication")
        .WithTags("Applications")
        .WithOpenApi();
    }
}
