
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using AuthNetExample.Features.Auth.Services;
using FluentValidation;
using Namespace.Features.Shared.Api.ExceptionHandlers;
using AuthNetExample.Features.Shared.Seeders;
using Features.JobPosting.Endpoints;
using Features.JobPosting.Services;
using AuthNetExample.Features.Applications.Endpoints;
using Features.Auth.Endpoints;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalSerializationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services
    .AddOptions<DatabaseSettings>()
    .Bind(builder.Configuration.GetSection("DatabaseSettings"));

builder.Services
    .AddOptions<OAuthGithubSettings>()
    .Bind(builder.Configuration.GetSection("OAuthSettings:Github"));

builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityServices()
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});


builder.Services
    .AddAuthentication()
    .AddGitHubProvider(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JobPostingService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// Auth Endpoints
app.AddLoginUserEndpoint();
app.AddRefreshTokenEndpoint();
app.AddWhoAmIEndpoint();
app.AddSignInWithGithubEndpoint();

// Job Posting Endpoints
app.AddGetJobOfferByIdEndpoint();
app.AddGetJobOffersEndpoint();
app.AddPostJobOfferEndpoint();
app.AddUpdateJobOfferEndpoint();
app.AddDeleteJobOfferEndpoint();
app.AddOpenGithubLoginPageEndpoint();

// Application Endpoints
app.AddSubmitApplicationEndpoint();
app.AddGetApplicationsEndpoint();
app.AddGetApplicationsByJobOfferEndpoint();
app.AddUpdateApplicationStatusEndpoint();
app.AddDeleteApplicationEndpoint();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}

app.Run();