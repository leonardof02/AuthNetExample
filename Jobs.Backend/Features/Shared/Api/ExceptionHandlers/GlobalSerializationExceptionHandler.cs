namespace Namespace.Features.Shared.Api.ExceptionHandlers;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

public class GlobalSerializationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();

        if (exception is BadHttpRequestException || exception is System.Text.Json.JsonException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Invalid JSON Format";
            problemDetails.Detail = "The JSON format is incorrect or contains invalid data types.";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
        else
        {
            return false;
        }
    }
}