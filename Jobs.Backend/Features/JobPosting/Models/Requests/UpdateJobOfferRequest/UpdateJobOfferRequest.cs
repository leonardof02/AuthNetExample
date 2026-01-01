public record UpdateJobOfferRequest(
    string Title,
    string Description,
    string Location,
    int MinSalary,
    int MaxSalary
);