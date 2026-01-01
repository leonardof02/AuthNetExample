using System.Security.Claims;
using Jobs.Backend.Features.Profile.Services;
using Features.Companies.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

public static class AddCompanyToDbContext
{
    public static WebApplication AddCompanyToRecruiterEndpoint(this WebApplication app)
    {
        app.MapPost("/profile/recruiter/companies", async (string recruiterId, CreateCompanyRequest request, CompanyService companyService, ClaimsPrincipal user) =>
        {
            var recruiter = user;
            var company = await companyService.CreateCompanyAsync(request);
        })
        .AddEndpointFilter<ValidationFilter<CreateCompanyRequest>>()
        .RequireAuthorization(
            new AuthorizeAttribute
            {
                Roles = AppRoles.Recruiter,
                AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
            }
        );

        return app;
    }
}
