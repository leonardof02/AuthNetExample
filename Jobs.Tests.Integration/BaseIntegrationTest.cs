using Features.Shared.Persistence;
using Jobs.Tests.Integration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Jobs.Tests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<ApiFactory>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly RoleManager<IdentityRole> RoleManager;
    protected readonly UserManager<ApplicationUser> UserManager;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(ApiFactory factory)
    {
        Client = factory.CreateClient();
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();

        // Seed here if needed
        string[] roles = { AppRoles.Recruiter, AppRoles.Applicant };

        foreach (var roleName in roles)
        {
            if (!RoleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                RoleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }
        }
    }

    public void Dispose()
    {
        Scope.Dispose();
        DbContext.Dispose();
    }
}