using Features.Companies.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Jobs.Backend.Features.Profile.Endpoints;

public static class UpdateCompanyAsync
{
    public static WebApplication UpdateCompanyEndpoint(this WebApplication app)
    {
        app.MapPatch("/profile/recruiter/companies/{companyId:int}", async (int companyId, UpdateCompanyRequest request, CompanyService companyService) =>
        {
            await companyService.UpdateCompanyAsync(companyId, request);
        })
        .AddEndpointFilter<ValidationFilter<UpdateCompanyRequest>>()
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