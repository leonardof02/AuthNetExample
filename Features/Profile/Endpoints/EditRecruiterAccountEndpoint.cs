using AuthNetExample.Features.Profile.Services;
using Features.Profile.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Features.Profile.Endpoints;

public static class EditRecruiterAccountEndpoint
{
    public static void AddEditRecruiterAccountEndpoint(this WebApplication app)
    {
        app.MapPatch("/profile/recruiter/{profileId:int}", async (
            int profileId,
            UpdateRecruiterRequest request,
            RecruiterAccountsService recruiterAccountsService
        ) =>
        {
            var updatedProfile = await recruiterAccountsService.UpdateRecruiterProfileAsync(request, profileId);
            return Results.Ok(updatedProfile);
        })
        .AddEndpointFilter<ValidationFilter<UpdateRecruiterRequest>>()
        .RequireAuthorization(new AuthorizeAttribute
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = AppRoles.Recruiter
        });
    }
}