using Api.Dtos;
using Application.Scooters.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("scooters")]
[ApiController]
public class ScootersController : ControllerBase
{
    private readonly IScooterService _scooterService;

    public ScootersController(IScooterService scooterService)
    {
        _scooterService = scooterService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ScooterDto>>> GetScooters(
        CancellationToken cancellationToken)
    {
        var scooters = await _scooterService.GetScooters(cancellationToken);

        var result = scooters
            .Select(ScooterDto.FromDomainModel)
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScooterDto>> GetScooter(
        Guid id,
        CancellationToken cancellationToken)
    {
        var scooter = await _scooterService.GetScooter(id, cancellationToken);

        if (scooter is null)
        {
            return NotFound();
        }

        return Ok(ScooterDto.FromDomainModel(scooter));
    }

    [HttpPost]
    public async Task<ActionResult<ScooterDto>> CreateScooter(
        [FromBody] CreateScooterDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var scooter = await _scooterService.Add(
                request.Model,
                request.SerialNumber,
                request.BatteryPercent,
                request.PricePerMinute,
                request.IsAvailable,
                request.LastServiceDate,
                cancellationToken);

            var dto = ScooterDto.FromDomainModel(scooter);

            return CreatedAtAction(
                nameof(GetScooter),
                new { id = dto.Id },
                dto);
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
            var scooter = await _scooterService.Update(
                id,
                request.Model,
                request.SerialNumber,
                request.BatteryPercent,
                request.PricePerMinute,
                request.IsAvailable,
                request.LastServiceDate,
                cancellationToken);

            if (scooter is null)
            {
                return NotFound();
            }

            return Ok(ScooterDto.FromDomainModel(scooter));
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteScooter(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _scooterService.Delete(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}