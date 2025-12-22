using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthNetExample.Features.Profile.Endpoints;

public record ApplicantProfileRequest(
    string FullName,
    string ProfileTitle,
    string Bio,
    string CvUrl,
    string WebsiteUrl,
    string ContactPhone,
    string ContactEmail
);

public static class CreateApplicantAccount
{
    public static void AddCreateApplicantAccountEndpoint(this WebApplication app)
    {
        app.MapPost("/profile/applicant", async (
            ApplicantProfile profile,
            ApplicationDbContext dbContext
        ) =>
        {
            dbContext.ApplicantProfiles.Add(profile);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/profile/applicant/{profile.Id}", profile);
        })
        .RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "applicant"
        });
    }
}