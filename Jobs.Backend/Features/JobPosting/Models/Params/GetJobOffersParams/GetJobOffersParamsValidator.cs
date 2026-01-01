using FluentValidation;

namespace Features.JobPosting.Models.GetJobOffersParams; 

public class GetJobOffersParamsValidator : AbstractValidator<GetJobOffersParams>
{
    public GetJobOffersParamsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

        RuleFor(x => x.MinSalaryFilter)
            .GreaterThanOrEqualTo(0).When(x => x.MinSalaryFilter.HasValue)
            .WithMessage("Minimum salary filter must be non-negative.");

        RuleFor(x => x.MaxSalaryFilter)
            .GreaterThanOrEqualTo(0).When(x => x.MaxSalaryFilter.HasValue)
            .WithMessage("Maximum salary filter must be non-negative.");

        RuleFor(x => x)
            .Must(x => !x.MinSalaryFilter.HasValue || !x.MaxSalaryFilter.HasValue || x.MinSalaryFilter <= x.MaxSalaryFilter)
            .WithMessage("Minimum salary filter cannot be greater than maximum salary filter.");
    }
}