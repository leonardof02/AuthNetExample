using FluentValidation;

namespace Features.JobPosting.Models.PostJobOfferRequest;

public class PostJobOfferRequestValidator : AbstractValidator<PostJobOfferRequest>
{
    public PostJobOfferRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(x => x.MinSalary)
            .NotEmpty().WithMessage("MinSalary is required.");

        RuleFor(x => x.MaxSalary)
            .NotEmpty().WithMessage("MaxSalary is required.")
            .GreaterThanOrEqualTo(x => x.MinSalary).WithMessage("MaxSalary must be greater than or equal to MinSalary.");
    }
}