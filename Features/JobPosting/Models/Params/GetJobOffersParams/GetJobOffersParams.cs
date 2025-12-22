namespace Features.JobPosting.Models.GetJobOffersParams;

public record GetJobOffersParams
(
    int PageNumber = 1,
    int PageSize = 10,
    string? EmployerIdFilter = null,
    string? LocationFilter = null,
    int? MinSalaryFilter = null,
    int? MaxSalaryFilter = null
);