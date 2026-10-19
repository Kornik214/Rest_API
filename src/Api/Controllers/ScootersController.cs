using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Scooters.Commands;
using Domain.Scooters;
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
        var scooter = await scooterQueries.GetById(new ScooterId(id), cancellationToken);

        return scooter.Match<ActionResult<ScooterDto>>(
            found => Ok(ScooterDto.FromDomainModel(found)),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<ScooterDto>> CreateScooter(
        [FromBody] CreateScooterDto request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateScooterCommand
        {
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            BatteryPercent = request.BatteryPercent,
            PricePerMinute = request.PricePerMinute,
            IsAvailable = request.IsAvailable,
            LastServiceDate = request.LastServiceDate
        }, cancellationToken);

        return result.Match<ActionResult<ScooterDto>>(
            scooter => CreatedAtAction(nameof(GetScooter), new { id = scooter.Id.Value },
                ScooterDto.FromDomainModel(scooter)),
            error => error.ToObjectResult());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ScooterDto>> UpdateScooter(
        Guid id,
        [FromBody] UpdateScooterDto request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateScooterCommand
        {
            ScooterId = id,
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            BatteryPercent = request.BatteryPercent,
            PricePerMinute = request.PricePerMinute,
            IsAvailable = request.IsAvailable,
            LastServiceDate = request.LastServiceDate
        }, cancellationToken);

        return result.Match<ActionResult<ScooterDto>>(
            scooter => Ok(ScooterDto.FromDomainModel(scooter)),
            error => error.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteScooter(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteScooterCommand { ScooterId = id }, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            error => error.ToObjectResult());
    }
}
