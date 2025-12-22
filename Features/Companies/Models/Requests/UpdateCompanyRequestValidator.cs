using FluentValidation;

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest> 
{
    public UpdateCompanyRequestValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Company ID must be a positive integer.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Website)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("Invalid website URL format.")
            .When(x => !string.IsNullOrEmpty(x.Website));
    }
}