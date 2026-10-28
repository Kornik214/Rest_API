using Api.Contracts;
using Api.Extensions;
using Application.Scooters;
using Application.Scooters.Commands;
using Domain.Scooters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScootersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IScooterQueries _queries;

    public ScootersController(ISender sender, IScooterQueries queries)
    {
        _sender = sender;
        _queries = queries;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var scooters = await _queries.GetAllAsync(cancellationToken);
        var response = scooters.Select(ScooterResponse.FromDomain);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var option = await _queries.GetByIdAsync(new ScooterId(id), cancellationToken);

        return option.Match<IActionResult>(
            Some: scooter => Ok(ScooterResponse.FromDomain(scooter)),
            None: () => NotFound(new { Error = $"Самокат з ідентифікатором {id} не знайдено." })
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScooterRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateScooterCommand(
            request.SerialNumber,
            request.BatteryLevel,
            request.IsAvailable,
            request.LastMaintenanceDate);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            Right: scooter => CreatedAtAction(nameof(GetById), new { id = scooter.Id.Value }, ScooterResponse.FromDomain(scooter)),
            Left: error => error.ToActionResult()
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScooterRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateScooterCommand(
            id,
            request.SerialNumber,
            request.BatteryLevel,
            request.IsAvailable,
            request.LastMaintenanceDate);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            Right: scooter => Ok(ScooterResponse.FromDomain(scooter)),
            Left: error => error.ToActionResult()
        );
    }
}