using System.IdentityModel.Tokens.Jwt;
using Amazon.Runtime;
using Amazon.S3;
using Jobs.Backend.Features.Applications.Endpoints;
using Jobs.Backend.Features.Auth.Services;
using Jobs.Backend.Features.Notifications.Endpoints;
using Jobs.Backend.Features.Notifications.Services;
using Jobs.Backend.Features.Notifications.Workers;
using Jobs.Backend.Features.Profile.Endpoints;
using Jobs.Backend.Features.Profile.Services;
using Jobs.Backend.Features.Shared.Seeders;
using Features.Auth.Endpoints;
using Features.Auth.Extensions;
using Features.JobPosting.Endpoints;
using Features.JobPosting.Services;
using Features.Profile.Endpoints;
using Features.Profile.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Namespace.Features.Shared.Api.ExceptionHandlers;
using Scalar.AspNetCore;
using Telegram.Bot;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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

builder.Services
    .AddOptions<S3Config>()
    .Bind(builder.Configuration.GetSection("AWS"));

builder.Services.AddOptions<TelegramConfig>()
    .Bind(builder.Configuration.GetSection("Telegram"));

builder.Services.AddSingleton<IAmazonS3>(sc =>
{
    var configuration = sc.GetRequiredService<IConfiguration>();
    var awsConfiguration = configuration.GetSection("AWS").Get<S3Config>();
    var region = Amazon.RegionEndpoint.GetBySystemName(awsConfiguration.Region);

    if (awsConfiguration?.ServiceURL is null)
    {
        if (!string.IsNullOrEmpty(awsConfiguration?.AccessKey) && 
            !string.IsNullOrEmpty(awsConfiguration?.SecretKey))
        {
            var credentials = new BasicAWSCredentials(
                awsConfiguration.AccessKey, 
                awsConfiguration.SecretKey
            );
            return new AmazonS3Client(credentials, region);
        }
        
        return new AmazonS3Client(region);
    }

    var s3Config = new AmazonS3Config
    {
        AuthenticationRegion = region.SystemName,
        ServiceURL = awsConfiguration.ServiceURL,
        ForcePathStyle = awsConfiguration.ForcePathStyle,
        UseArnRegion = awsConfiguration.UseArnRegion,
        RegionEndpoint = region
    };

    if (!string.IsNullOrEmpty(awsConfiguration?.AccessKey) && 
        !string.IsNullOrEmpty(awsConfiguration?.SecretKey))
    {
        var credentials = new BasicAWSCredentials(
            secretKey: awsConfiguration.SecretKey,
            accessKey: awsConfiguration.AccessKey,
            accountId: "000000000000"
        );
        return new AmazonS3Client(credentials, s3Config);
    }

    return new AmazonS3Client(s3Config);
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetSection("DatabaseSettings:ConnectionString").Value);
});

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

var botToken = builder.Configuration["Telegram:BotToken"]
               ?? throw new Exception("Telegram Token no configurado");

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
builder.Services.AddScoped<TelegramBotService>();

builder.Services.AddLogging();
builder.Services.AddHostedService<TelegramNotificationBackgroundService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JobPostingService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<ApplicantAccountService>();
builder.Services.AddScoped<RecruiterAccountsService>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddSingleton<MemoryCacheService>();
builder.Services.AddHostedService<TelegramNotificationBackgroundService>();
builder.Services.AddScoped<CvStorageService>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("AzureStorage:ConnectionString").Value);
});


builder.Services.AddOpenApi();

builder.Services.AddAntiforgery();

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

app.UseAntiforgery();

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
app.AddGetApplicationsByJobOfferEndpoint();
app.AddUpdateApplicationStatusEndpoint();
app.AddDeleteApplicationEndpoint();

// Acounts
app.AddCreateApplicantAccountEndpoint();
app.AddCreateRecruiterAccountEndpoint();
app.AddEditApplicantAccountEndpoint();
app.AddEditRecruiterAccountEndpoint();
app.AddCompanyToRecruiterEndpoint();
app.UpdateCompanyEndpoint();

// Notifications
app.AddGetTelegramLinkEndpoint();

// Cv Management
app.MapUploadCvEndpoint();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}

app.Run();

public partial class Program { }