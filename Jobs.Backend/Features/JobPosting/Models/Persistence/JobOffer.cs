namespace Features.JobPosting.Models;

public class JobOffer
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public int MinSalary { get; set; }
    public int MaxSalary { get; set; }

    public string EmployerId { get; set; }
    public DateTime PostedDate { get; set; }

    public JobOffer(string title, string description, string location, int minSalary, int maxSalary, string employerId)
    {
        Title = title;
        Description = description;
        Location = location;
        MinSalary = minSalary;
        MaxSalary = maxSalary;
        EmployerId = employerId;
        PostedDate = DateTime.UtcNow;
    }

}