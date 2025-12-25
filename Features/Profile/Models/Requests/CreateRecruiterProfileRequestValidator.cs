
using FluentValidation;

namespace Features.Profile.Models.Requests;

public class CreateRecruiterProfileRequestValidator : AbstractValidator<CreateRecruiterProfileRequest>
{
    public CreateRecruiterProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.ProfileTitle)
            .MaximumLength(100).WithMessage("Profile title cannot exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.");

        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.ContactPhone))
            .WithMessage("Contact phone must be a valid phone number.");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail))
            .WithMessage("Contact email must be a valid email address.");
    }
}