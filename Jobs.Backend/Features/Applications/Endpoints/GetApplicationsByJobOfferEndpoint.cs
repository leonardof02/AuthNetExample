using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobs.Backend.Features.Applications.Endpoints;

public static class GetApplicationsByJobOfferEndpoint
{
    public static void AddGetApplicationsByJobOfferEndpoint(this WebApplication app)
    {
        app.MapGet("/api/joboffers/{jobOfferId}/applications", async (
            int jobOfferId,
            [FromServices] ApplicationService applicationService,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            ) =>
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return Results.BadRequest(new { message = "Page number and page size must be greater than 0" });
            }

            var applications = await applicationService.GetApplicationsByJobOfferIdAsync(jobOfferId, pageNumber, pageSize);

            return Results.Ok(applications);
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
