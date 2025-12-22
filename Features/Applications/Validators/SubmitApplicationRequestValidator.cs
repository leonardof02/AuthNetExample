using FluentValidation;
using AuthNetExample.Features.Applications.Models.Requests;

namespace AuthNetExample.Features.Applications.Validators;

public class SubmitApplicationRequestValidator : AbstractValidator<SubmitApplicationRequest>
{
    public SubmitApplicationRequestValidator()
    {
        RuleFor(x => x.JobOfferId)
            .NotEmpty()
            .WithMessage("El ID de la oferta es requerido")
            .GreaterThan(0)
            .WithMessage("El ID de la oferta debe ser mayor que 0");
    }
}
