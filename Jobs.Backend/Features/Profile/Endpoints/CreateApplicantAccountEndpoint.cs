using Features.Profile.Models.Requests;
using Features.Profile.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Jobs.Backend.Features.Profile.Endpoints;

public static class CreateApplicantAccount
{
    public static void AddCreateApplicantAccountEndpoint(this WebApplication app)
    {
        app.MapPost("/profile/applicant", async (
            CreateApplicantProfileRequest request,
            ApplicantAccountService applicantAccountService
        ) =>
        {
            var profile =  await applicantAccountService.CreateApplicantProfileAsync(request);
            return Results.Created($"/profile/applicant/{profile.Id}", profile);
        })
        .AddEndpointFilter<ValidationFilter<CreateApplicantProfileRequest>>()
        .RequireAuthorization(new AuthorizeAttribute
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "applicant",
            Policy = "FirstTimeUsingTheApp"
        });
    }
}