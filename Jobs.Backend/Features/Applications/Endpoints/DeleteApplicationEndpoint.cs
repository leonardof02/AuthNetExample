using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobs.Backend.Features.Applications.Endpoints;

public static class DeleteApplicationEndpoint
{
    public static void AddDeleteApplicationEndpoint(this WebApplication app)
    {
        app.MapDelete("/joboffers/{jobOfferId}/applications/{id}", async (
            int jobOfferId,
            int id,
            [FromServices] ApplicationService applicationService
        ) =>
        {
            var deletedId = await applicationService.DeleteApplicationAsync(id);
            return Results.NoContent();
        })
        .RequireAuthorization(
            new AuthorizeAttribute
            {
                Roles = AppRoles.Applicant,
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
            }
        );
    }
}
