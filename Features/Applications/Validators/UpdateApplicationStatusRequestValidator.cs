using FluentValidation;
using AuthNetExample.Features.Applications.Models.Requests;

namespace AuthNetExample.Features.Applications.Validators;

public class UpdateApplicationStatusRequestValidator : AbstractValidator<UpdateApplicationStatusRequest>
{
    public UpdateApplicationStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("El estado es requerido")
            .Must(status => new[] { "Pending", "Reviewed", "Accepted", "Rejected" }.Contains(status))
            .WithMessage("El estado debe ser uno de: Pending, Reviewed, Accepted, Rejected");
    }
}
