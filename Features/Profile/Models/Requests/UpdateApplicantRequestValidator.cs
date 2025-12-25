using Features.Profile.Models.Requests;
using FluentValidation;

public class UpdateApplicantRequestValidator : AbstractValidator<UpdateApplicantRequest>
{
    public UpdateApplicantRequestValidator()
    {

        RuleFor(x => x.FullName)
            .NotEmpty()
            .When(x => x.FullName != null)
            .WithMessage("FullName cannot be empty.")
            .MaximumLength(100)
            .WithMessage("FullName cannot exceed 100 characters.");

        RuleFor(x => x.ProfileTitle)
            .MaximumLength(100)
            .When(x => x.ProfileTitle != null)
            .WithMessage("ProfileTitle cannot exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .When(x => x.Bio != null)
            .WithMessage("Bio cannot exceed 1000 characters.");


        RuleFor(x => x.ContactEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.ContactEmail))
            .WithMessage("ContactEmail must be a valid email address.");

        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrEmpty(x.ContactPhone))
            .WithMessage("ContactPhone must be a valid phone number.");

        RuleFor(x => x.WebsiteUrl)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .When(x => !string.IsNullOrEmpty(x.WebsiteUrl))
            .WithMessage("WebsiteUrl must be a valid URL.");
    }
}