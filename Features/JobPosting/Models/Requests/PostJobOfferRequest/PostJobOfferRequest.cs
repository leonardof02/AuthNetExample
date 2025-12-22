namespace Features.JobPosting.Models.PostJobOfferRequest;

public record PostJobOfferRequest(
    string Title,
    string Description,
    string Location,
    int MinSalary,
    int MaxSalary
);