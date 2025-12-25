using FluentValidation;

namespace Features.Profile.Models.Requests;

public class CreateApplicantProfileRequestValidator : AbstractValidator<CreateApplicantProfileRequest>
{
    public CreateApplicantProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.ProfileTitle)
            .NotEmpty().WithMessage("Profile title is required.")
            .MaximumLength(100).WithMessage("Profile title cannot exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.");

        RuleFor(x => x.ContactPhone)
            .MaximumLength(15).WithMessage("Contact phone cannot exceed 15 characters.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required.")
            .EmailAddress().WithMessage("Contact email must be a valid email address.");
    }
}