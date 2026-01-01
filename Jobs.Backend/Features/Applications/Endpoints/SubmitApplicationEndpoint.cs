using Jobs.Backend.Features.Applications.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobs.Backend.Features.Applications.Endpoints;

public static class SubmitApplicationEndpoint
{
    public static void AddSubmitApplicationEndpoint(this WebApplication app)
    {
        app.MapPost("/joboffers/{jobOfferId}/applications", async (
            int jobOfferId,
            [FromServices] ApplicationService applicationService) =>
        {
            var application = await applicationService.SubmitApplicationAsync(jobOfferId);
            return Results.Created($"/joboffers/{jobOfferId}/applications/{application.Id}", new
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
                Roles = AppRoles.Applicant,
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
            }
        );
    }
}
