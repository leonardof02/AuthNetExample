using AuthNetExample.Features.Profile.Services;
using Features.Profile.Models.Requests;
using Features.Profile.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace AuthNetExample.Features.Profile.Endpoints;

public static class CreateRecruiterAccount
{
    public static void AddCreateRecruiterAccountEndpoint(this WebApplication app)
    {
        app.MapPost("/profile/recruiter", async (
            CreateRecruiterProfileRequest request,
            RecruiterAccountsService recruiterAccountService
        ) =>
        {
            var profile =  await recruiterAccountService.CreateRecruiterAccountAsync(request);
            return Results.Created($"/profile/recruiter/{profile.Id}", profile);
        })
        .AddEndpointFilter<ValidationFilter<CreateRecruiterProfileRequest>>()
        .RequireAuthorization(new AuthorizeAttribute
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = AppRoles.Recruiter,
            Policy = "FirstTimeUsingTheApp"
        });
    }
}