using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Modules.Errors;

public class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException exception)
        {
            return;
        }

        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.ErrorMessage).ToArray());

        context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errors));
        context.ExceptionHandled = true;
    }
}
