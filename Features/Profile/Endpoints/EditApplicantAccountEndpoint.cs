using System.Security.Claims;
using AuthNetExample.Features.Auth.Models.Constants;
using Features.Profile.Models.Requests;
using Features.Profile.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace AuthNetExample.Features.Profile.Endpoints;

public static class EditApplicantAccountEndpoint
{
    public static void AddEditApplicantAccountEndpoint(this WebApplication app)
    {
        app.MapPatch("/profile/applicant/{profileId:int}", async (
            int profileId,
            UpdateApplicantRequest request,
            ApplicantAccountService applicantAccountsService,
            ClaimsPrincipal user
        ) =>
        {
            var updatedProfile = await applicantAccountsService.UpdateApplicantProfileAsync(request, profileId);
            return Results.Ok(updatedProfile);
        })
        .AddEndpointFilter<ValidationFilter<UpdateApplicantRequest>>()
        .RequireAuthorization(new AuthorizeAttribute
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = AppRoles.Applicant
        });
    }
}
