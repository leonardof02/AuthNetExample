using Microsoft.AspNetCore.Identity;
using Features.JobPosting.Models;
using Features.Shared.Persistence;

namespace AuthNetExample.Features.Shared.Seeders;

public class DatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public DatabaseSeeder(
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedJobOffersAsync();
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

    private async Task SeedUsersAsync()
    {
        var testUsers = new List<(string Email, string Password)>
        {
            ("employer@example.com", "Employer@123"),
            ("recruiter@example.com", "Recruiter@123"),
            ("john.doe@example.com", "John@123456"),
            ("jane.smith@example.com", "Jane@123456"),
            ("carlos.garcia@example.com", "Carlos@123456"),
            ("maria.lopez@example.com", "Maria@123456"),
            ("pedro.martinez@example.com", "Pedro@123456"),
            ("ana.fernandez@example.com", "Ana@123456")
        };

        foreach (var (email, password) in testUsers)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(newUser, password);
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Error creating user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                else
                {
                    Console.WriteLine($"✓ User created: {email}");
                }
            }
        }
    }

    private async Task SeedJobOffersAsync()
    {
        if (_dbContext.JobOffers.Any())
        {
            Console.WriteLine("Job offers already exist, skipping seeding.");
            return;
        }

        var employerId = (await _userManager.FindByEmailAsync("employer@example.com"))?.Id;
        var recruiterId = (await _userManager.FindByEmailAsync("recruiter@example.com"))?.Id;

        if (string.IsNullOrEmpty(employerId) || string.IsNullOrEmpty(recruiterId))
        {
            Console.WriteLine("Employer or Recruiter user not found. Skipping job offers seeding.");
            return;
        }

        var jobOffers = new List<JobOffer>
        {
            // Ofertas del Employer
            new JobOffer(
                "Senior Full Stack Developer",
                "We are looking for an experienced full stack developer with expertise in C# and React. You will be responsible for developing and maintaining our web applications. Must have 5+ years of experience.",
                "Madrid, Spain",
                45000,
                60000,
                employerId
            ),
            new JobOffer(
                "Backend Engineer - C#/.NET",
                "Join our growing backend team! We're seeking a talented backend engineer to work with our microservices architecture. Experience with Docker, Kubernetes, and Azure is a plus.",
                "Barcelona, Spain",
                40000,
                55000,
                employerId
            ),
            new JobOffer(
                "DevOps Engineer",
                "Help us build and maintain our cloud infrastructure. We use Azure, Docker, and Kubernetes. You'll work on CI/CD pipelines and infrastructure automation.",
                "Remote",
                38000,
                52000,
                employerId
            ),
            new JobOffer(
                "Frontend Developer - React",
                "Create beautiful and responsive user interfaces using React and TypeScript. Work with our design team to bring mockups to life. Must have 3+ years of React experience.",
                "Valencia, Spain",
                35000,
                48000,
                employerId
            ),
            new JobOffer(
                "Database Administrator",
                "Manage and optimize our SQL Server and PostgreSQL databases. Monitor performance, handle backups, and implement security measures. Requires expertise in database administration.",
                "Madrid, Spain",
                42000,
                56000,
                employerId
            ),

            // Ofertas del Recruiter
            new JobOffer(
                "Mobile Developer - iOS/Android",
                "Develop cross-platform mobile applications using Flutter or React Native. Join a startup that's disrupting the mobile industry. Experience with native development is a plus.",
                "Málaga, Spain",
                33000,
                48000,
                recruiterId
            ),
            new JobOffer(
                "QA Engineer - Automation",
                "Build and maintain automated test suites for our web and mobile applications. Experience with Selenium, Cypress, or similar frameworks required. Knowledge of Java or Python is preferred.",
                "Remote",
                28000,
                40000,
                recruiterId
            ),
            new JobOffer(
                "Solutions Architect",
                "Design cloud solutions for enterprise clients using Azure or AWS. Work with stakeholders to understand requirements and translate them into technical solutions. 8+ years experience required.",
                "Madrid, Spain",
                55000,
                75000,
                recruiterId
            ),
            new JobOffer(
                "Security Engineer",
                "Lead our security initiatives and implement best practices. Conduct security audits and penetration testing. CISSP or similar certification preferred.",
                "Barcelona, Spain",
                50000,
                68000,
                recruiterId
            ),
            new JobOffer(
                "Machine Learning Engineer",
                "Develop and deploy ML models for our data platform. Experience with Python, TensorFlow, and PyTorch. Knowledge of MLOps and model deployment is essential.",
                "Madrid, Spain",
                48000,
                65000,
                recruiterId
            ),
            new JobOffer(
                "Data Engineer",
                "Build data pipelines and ETL processes using Spark or similar tools. Design data warehouses and implement analytics solutions. SQL and Python expertise required.",
                "Remote",
                42000,
                58000,
                recruiterId
            ),
            new JobOffer(
                "Cloud Architect - AWS",
                "Design and implement scalable cloud solutions on AWS. Work with teams to migrate legacy systems to the cloud. AWS Solutions Architect certification required.",
                "Bilbao, Spain",
                52000,
                70000,
                recruiterId
            ),
            new JobOffer(
                "Technical Lead",
                "Lead a team of developers and drive technical excellence. Mentor junior developers and make architectural decisions. 6+ years of development experience required.",
                "Madrid, Spain",
                48000,
                62000,
                recruiterId
            )
        };

        await _dbContext.JobOffers.AddRangeAsync(jobOffers);
        await _dbContext.SaveChangesAsync();

        Console.WriteLine($"✓ {jobOffers.Count} job offers created successfully!");
    }
}
