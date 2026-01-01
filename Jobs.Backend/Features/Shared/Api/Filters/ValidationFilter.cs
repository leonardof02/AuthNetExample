
using FluentValidation;

public class ValidationFilter<T>() : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context
            .HttpContext
            .RequestServices
            .GetService<IValidator<T>>();

        if (validator != null)
        {
            var argument = context.Arguments.OfType<T>().FirstOrDefault();
            if (argument is null)
            {
                return Results.BadRequest(new { Errors = new[] { $"An object of type {typeof(T).Name} is required." } });
            }
            if (argument != null)
            {
                var validationResult = validator.Validate(argument);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => e.ErrorMessage);


                    return Results.BadRequest(new { Errors = errors });
                }
            }
        }

        return await next(context);
    }
}