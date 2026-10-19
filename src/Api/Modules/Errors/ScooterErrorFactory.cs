using Application.Scooters.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ScooterErrorFactory
{
    public static ObjectResult ToObjectResult(this ScooterException error) => new(error.Message)
    {
        StatusCode = error switch
        {
            ScooterAlreadyExistsException => StatusCodes.Status409Conflict,
            ScooterNotFoundException => StatusCodes.Status404NotFound,
            ScooterUnhandledException => StatusCodes.Status500InternalServerError,
            _ => throw new NotImplementedException("Scooter error handler is not implemented.")
        }
    };
}
