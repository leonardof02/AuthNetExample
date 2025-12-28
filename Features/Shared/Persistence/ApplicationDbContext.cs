using AuthNetExample.Features.Auth.Models;
using AuthNetExample.Features.Notifications.Models.Persistence;
using Features.JobPosting.Models;
using Features.Shared.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<JobOffer> JobOffers => Set<JobOffer>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<ApplicantProfile> ApplicantProfiles => Set<ApplicantProfile>();
    public DbSet<RecruiterProfile> RecruiterProfiles => Set<RecruiterProfile>();
    public DbSet<TelegramSuscription> TelegramSuscriptions => Set<TelegramSuscription>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token);

            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<JobOffer>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.EmployerId).IsRequired();
            entity.Property(e => e.Location).IsRequired();
            entity.Property(e => e.PostedDate).IsRequired();
            entity.Property(e => e.MinSalary).IsRequired();
            entity.Property(e => e.MaxSalary).IsRequired();

            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(e => e.EmployerId)
                  .IsRequired();
        });

        builder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.JobOfferId).IsRequired();
            entity.Property(e => e.ApplicantId).IsRequired();
            entity.Property(e => e.AppliedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            entity.HasOne<JobOffer>()
                  .WithMany()
                  .HasForeignKey(e => e.JobOfferId)
                  .IsRequired();
        });

        builder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description);
            entity.Property(e => e.Email);
            entity.Property(e => e.PhoneNumber);
            entity.Property(e => e.Website);

            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(e => e.RecruiterId)
                  .IsRequired();
        });

        builder.Entity<ApplicantProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.CvUrl);
            entity.Property(e => e.WebsiteUrl);

            entity.HasOne<ApplicationUser>()
                  .WithOne()
                  .HasForeignKey<ApplicantProfile>(e => e.UserId)
                  .IsRequired();
        });

        builder.Entity<RecruiterProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName).IsRequired();
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne<ApplicationUser>()
                  .WithOne()
                  .HasForeignKey<RecruiterProfile>(e => e.UserId)
                  .IsRequired();
        });

        builder.Entity<TelegramSuscription>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ChatId });

            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.ChatId).IsRequired();

            entity.HasOne<ApplicationUser>()
                  .WithOne()
                  .HasForeignKey<TelegramSuscription>(e => e.UserId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ChatId).IsUnique();
        });
    }
}