using Domain.Scooters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ErrorExtensions
{
    public static IActionResult ToActionResult(this ScooterError error)
    {
        return error switch
        {
            ScooterAlreadyExistsError => new ConflictObjectResult(new { Error = error.Message }), // 409
            ScooterNotFoundError => new NotFoundObjectResult(new { Error = error.Message }),      // 404
            _ => new ObjectResult(new { Error = error.Message }) { StatusCode = StatusCodes.Status500InternalServerError } // 500
        };
    }
}