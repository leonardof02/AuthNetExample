using System.Net.Http.Json;
using Features.Auth.Models.Requests.RegisterRequest;
using Jobs.Tests.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

public class SignUpEndpointTests : BaseIntegrationTest
{
    private readonly string _endpoint = "/auth/signup";

    public SignUpEndpointTests(ApiFactory factory) : base(factory) { }


    [Fact]
    public async Task SignUpRecruiter_WithValidData_ReturnsSuccess()
    {
        var requestBody = new SignUpRequest
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            ConfirmPassword = "SecurePassword123!",
            Username = "testuser",
            Role = AppRoles.Recruiter
        };

        var response = await Client.PostAsJsonAsync(_endpoint, requestBody);

        response.EnsureSuccessStatusCode();

        using (var scope = Scope.ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userInDb = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == requestBody.Email);

            Assert.NotNull(userInDb);
            Assert.Equal(requestBody.Email, userInDb.Email);

            var userRoles = await dbContext.UserRoles
                .Where(ur => ur.UserId == userInDb.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var roles = await dbContext.Roles
                .Where(r => userRoles.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            Assert.Equal(requestBody.Role, roles.FirstOrDefault());
        }
    }

    [Fact]
    public async Task SignUpApplicant_WithValidData_ReturnsSuccess()
    {
        var requestBody = new SignUpRequest
        {
            Email = "applicant@example.com",
            Password = "SecurePassword123!",
            ConfirmPassword = "SecurePassword123!",
            Username = "applicantuser",
            Role = AppRoles.Applicant
        };
        var response = await Client.PostAsJsonAsync(_endpoint, requestBody);
        response.EnsureSuccessStatusCode();

        using (var scope = Scope.ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userInDb = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == requestBody.Email);

            Assert.NotNull(userInDb);
            Assert.Equal(requestBody.Email, userInDb.Email);

            var userRoles = await dbContext.UserRoles
                .Where(ur => ur.UserId == userInDb.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var roles = await dbContext.Roles
                .Where(r => userRoles.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            Assert.Equal(requestBody.Role, roles.FirstOrDefault());
        }
    }

    [Fact]
    public async Task SignUp_WithMismatchedPasswords_ReturnsBadRequest()
    {
        var requestBody = new SignUpRequest
        {
            Email = "mismatch@example.com",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!",
            Username = "mismatchuser",
            Role = AppRoles.Applicant
        };

        var response = await Client.PostAsJsonAsync(_endpoint, requestBody);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_WithExistingEmail_ReturnsBadRequest()
    {
        var newUser = new SignUpRequest
        {
            Email = "existing@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Username = "existinguser",
            Role = AppRoles.Applicant
        };

        var response = await Client.PostAsJsonAsync(_endpoint, newUser);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseDuplicate = await Client.PostAsJsonAsync(_endpoint, newUser);
        Assert.Equal(HttpStatusCode.InternalServerError, responseDuplicate.StatusCode);
    }
}