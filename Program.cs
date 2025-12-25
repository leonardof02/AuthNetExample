

using AuthNetExample.Features.Applications.Endpoints;
using AuthNetExample.Features.Auth.Services;
using AuthNetExample.Features.Profile.Endpoints;
using AuthNetExample.Features.Profile.Services;
using AuthNetExample.Features.Shared.Seeders;
using Features.Auth.Endpoints;
using Features.Auth.Extensions;
using Features.JobPosting.Endpoints;
using Features.JobPosting.Services;
using Features.Profile.Endpoints;
using Features.Profile.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Namespace.Features.Shared.Api.ExceptionHandlers;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddIdentityServices();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
})
.AddGitHubProvider(builder.Configuration)
.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddFirstTimeUsingTheAppPolicyService();
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JobPostingService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<ApplicantAccountService>();
builder.Services.AddScoped<RecruiterAccountsService>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddHttpContextAccessor();

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

// app.UseHttpsRedirection();


// Auth Endpoints
app.AddLoginUserEndpoint();
app.AddRefreshTokenEndpoint();
app.AddWhoAmIEndpoint();
app.AddSignInWithGithubEndpoint();
app.MapRegisterUserEndpoint();

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

// Acounts
app.AddCreateApplicantAccountEndpoint();
app.AddCreateRecruiterAccountEndpoint();
app.AddEditApplicantAccountEndpoint();
app.AddEditRecruiterAccountEndpoint();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}

app.Run();