using Api.Dtos;
using Application.Common.Interfaces.Queries;
using Application.Scooters.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("scooters")]
[ApiController]
public class ScootersController(ISender sender, IScooterQueries scooterQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ScooterDto>>> GetScooters(
        CancellationToken cancellationToken)
    {
        var scooters = await scooterQueries.GetAll(cancellationToken);
        return Ok(scooters.Select(ScooterDto.FromDomainModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScooterDto>> GetScooter(
        Guid id,
        CancellationToken cancellationToken)
    {
        var scooter = await scooterQueries.GetById(id, cancellationToken);
        return scooter is null ? NotFound() : Ok(ScooterDto.FromDomainModel(scooter));
    }

    [HttpPost]
    public async Task<ActionResult<ScooterDto>> CreateScooter(
        [FromBody] CreateScooterDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var scooter = await sender.Send(new CreateScooterCommand
            {
                Model = request.Model,
                SerialNumber = request.SerialNumber,
                BatteryPercent = request.BatteryPercent,
                PricePerMinute = request.PricePerMinute,
                IsAvailable = request.IsAvailable,
                LastServiceDate = request.LastServiceDate
            }, cancellationToken);

            var dto = ScooterDto.FromDomainModel(scooter);
            return CreatedAtAction(nameof(GetScooter), new { id = dto.Id }, dto);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ScooterDto>> UpdateScooter(
        Guid id,
        [FromBody] UpdateScooterDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var scooter = await sender.Send(new UpdateScooterCommand
            {
                ScooterId = id,
                Model = request.Model,
                SerialNumber = request.SerialNumber,
                BatteryPercent = request.BatteryPercent,
                PricePerMinute = request.PricePerMinute,
                IsAvailable = request.IsAvailable,
                LastServiceDate = request.LastServiceDate
            }, cancellationToken);

            return scooter is null ? NotFound() : Ok(ScooterDto.FromDomainModel(scooter));
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteScooter(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(new DeleteScooterCommand { ScooterId = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
