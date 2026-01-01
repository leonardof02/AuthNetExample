using Features.JobPosting.Models;
using Features.Shared.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jobs.Backend.Features.Shared.Seeders;

public class DatabaseSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private const string DefaultPassword = "ChangeMe123!";

    private record SeedUser(string Email, string FullName, string Role);


    public DatabaseSeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        var recruiters = await SeedRecruitersAsync();
        var applicants = await SeedApplicantsAsync();
        var jobOffers = await SeedJobOffersAsync(recruiters);
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "recruiter", "applicant" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    Console.WriteLine($"✓ Role created: {roleName}");
                }
                else
                {
                    Console.WriteLine($"Error creating role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

    private async Task<List<ApplicationUser>> SeedRecruitersAsync()
    {
        var recruiterSeeds = new[]
        {
            new SeedUser("ana.recruiter@example.com", "Ana Martinez", "recruiter"),
            new SeedUser("carlos.recruiter@example.com", "Carlos Gomez", "recruiter")
        };

        var recruiters = new List<ApplicationUser>();

        foreach (var seed in recruiterSeeds)
        {
            var user = await EnsureUserAsync(seed);
            await EnsureRecruiterProfileAsync(user, seed.FullName);
            recruiters.Add(user);
        }

        return recruiters;
    }

    private async Task<List<ApplicationUser>> SeedApplicantsAsync()
    {
        var applicantSeeds = new[]
        {
            new SeedUser("laura.applicant@example.com", "Laura Rivera", "applicant"),
            new SeedUser("miguel.applicant@example.com", "Miguel Santos", "applicant")
        };

        var applicants = new List<ApplicationUser>();

        foreach (var seed in applicantSeeds)
        {
            var user = await EnsureUserAsync(seed);
            await EnsureApplicantProfileAsync(user, seed.FullName);
            applicants.Add(user);
        }

        return applicants;
    }

    private async Task<List<JobOffer>> SeedJobOffersAsync(IEnumerable<ApplicationUser> recruiters)
    {
        var recruiterByEmail = recruiters
            .Where(r => !string.IsNullOrWhiteSpace(r.Email))
            .ToDictionary(r => r.Email!, r => r);

        var seeds = new[]
        {
            new { Title = "Backend Developer", Description = "API y servicios en .NET", Location = "Remoto", MinSalary = 60000, MaxSalary = 85000, RecruiterEmail = "ana.recruiter@example.com" },
            new { Title = "Frontend Engineer", Description = "UI con React y diseno accesible", Location = "Madrid", MinSalary = 50000, MaxSalary = 75000, RecruiterEmail = "ana.recruiter@example.com" },
            new { Title = "Data Analyst", Description = "Dashboarding y storytelling con datos", Location = "Barcelona", MinSalary = 45000, MaxSalary = 65000, RecruiterEmail = "carlos.recruiter@example.com" }
        };

        var created = new List<JobOffer>();

        foreach (var seed in seeds)
        {
            if (!recruiterByEmail.TryGetValue(seed.RecruiterEmail, out var recruiter))
            {
                continue;
            }

            var exists = await _dbContext.JobOffers
                .AnyAsync(j => j.Title == seed.Title && j.EmployerId == recruiter.Id);

            if (exists)
            {
                continue;
            }

            var jobOffer = new JobOffer(
                seed.Title,
                seed.Description,
                seed.Location,
                seed.MinSalary,
                seed.MaxSalary,
                recruiter.Id);

            _dbContext.JobOffers.Add(jobOffer);
            created.Add(jobOffer);
        }

        if (created.Count > 0)
        {
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"✓ Job offers created: {created.Count}");
        }

        var targetTitles = seeds.Select(s => s.Title).ToList();
        return await _dbContext.JobOffers.Where(j => targetTitles.Contains(j.Title)).ToListAsync();
    }

    // private async Task SeedJobApplicationsAsync(IEnumerable<JobOffer> jobOffers, IEnumerable<ApplicationUser> applicants)
    // {
    //     var jobOffersByTitle = jobOffers.ToDictionary(j => j.Title, j => j);
    //     var applicantsByEmail = applicants
    //         .Where(a => !string.IsNullOrWhiteSpace(a.Email))
    //         .ToDictionary(a => a.Email!, a => a);

    //     var seeds = new[]
    //     {
    //         new { JobTitle = "Backend Developer", ApplicantEmail = "laura.applicant@example.com", AppliedAt = DateTime.UtcNow.AddDays(-4) },
    //         new { JobTitle = "Frontend Engineer", ApplicantEmail = "miguel.applicant@example.com", AppliedAt = DateTime.UtcNow.AddDays(-3) },
    //         new { JobTitle = "Data Analyst", ApplicantEmail = "laura.applicant@example.com", AppliedAt = DateTime.UtcNow.AddDays(-2) }
    //     };

    //     var created = 0;

    //     foreach (var seed in seeds)
    //     {
    //         if (!jobOffersByTitle.TryGetValue(seed.JobTitle, out var jobOffer))
    //         {
    //             continue;
    //         }

    //         if (!applicantsByEmail.TryGetValue(seed.ApplicantEmail, out var applicant))
    //         {
    //             continue;
    //         }

    //         var exists = await _dbContext.JobApplications
    //             .AnyAsync(a => a.JobOfferId == jobOffer.Id && a.ApplicantId == applicant.Id);

    //         if (exists)
    //         {
    //             continue;
    //         }

    //         _dbContext.JobApplications.Add(new JobApplication
    //         {
    //             JobOfferId = jobOffer.Id,
    //             ApplicantId = applicant.Id,
    //             AppliedAt = seed.AppliedAt,
    //             Status = ApplicationStatus.Pending
    //         });

    //         created++;
    //     }

    //     if (created > 0)
    //     {
    //         await _dbContext.SaveChangesAsync();
    //         Console.WriteLine($"✓ Job applications created: {created}");
    //     }
    // }

    private async Task<ApplicationUser> EnsureUserAsync(SeedUser seed)
    {
        var existing = await _userManager.FindByEmailAsync(seed.Email);
        if (existing != null)
        {
            return existing;
        }

        var user = new ApplicationUser
        {
            Email = seed.Email,
            UserName = seed.Email,
            EmailConfirmed = true,
            IsOnboardingCompleted = true
        };

        var createResult = await _userManager.CreateAsync(user, DefaultPassword);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Could not seed user {seed.Email}: {errors}");
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, seed.Role);

        if (!addToRoleResult.Succeeded)
        {
            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Could not assign role {seed.Role} to {seed.Email}: {errors}");
        }

        Console.WriteLine($"✓ User created: {seed.Email} (role: {seed.Role})");

        return user;
    }

    private async Task EnsureRecruiterProfileAsync(ApplicationUser user, string fullName)
    {
        var exists = await _dbContext.RecruiterProfiles.AnyAsync(p => p.UserId == user.Id);
        if (exists)
        {
            return;
        }

        _dbContext.RecruiterProfiles.Add(new RecruiterProfile
        {
            UserId = user.Id,
            FullName = fullName,
            ContactEmail = user.Email ?? string.Empty
        });

        await _dbContext.SaveChangesAsync();
    }

    private async Task EnsureApplicantProfileAsync(ApplicationUser user, string fullName)
    {
        var exists = await _dbContext.ApplicantProfiles.AnyAsync(p => p.UserId == user.Id);
        if (exists)
        {
            return;
        }

        _dbContext.ApplicantProfiles.Add(new ApplicantProfile
        {
            UserId = user.Id,
            FullName = fullName,
            ContactEmail = user.Email ?? string.Empty
        });

        await _dbContext.SaveChangesAsync();
    }
}
