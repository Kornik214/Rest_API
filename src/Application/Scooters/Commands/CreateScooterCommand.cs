using Application.Common.Interfaces.Repositories;
using Domain.Scooters;
using MediatR;

namespace Application.Scooters.Commands;

public record CreateScooterCommand : IRequest<Scooter>
{
    public required string Model { get; init; }
    public required string SerialNumber { get; init; }
    public required int BatteryPercent { get; init; }
    public required decimal PricePerMinute { get; init; }
    public required bool IsAvailable { get; init; }
    public required DateTime LastServiceDate { get; init; }
}

public class CreateScooterCommandHandler(IScooterRepository scooterRepository)
    : IRequestHandler<CreateScooterCommand, Scooter>
{
    public async Task<Scooter> Handle(CreateScooterCommand request, CancellationToken cancellationToken)
    {
        var existingScooter = await scooterRepository.GetBySerialNumber(
            request.SerialNumber, cancellationToken);

        if (existingScooter is not null)
        {
            throw new ArgumentException(
                $"Scooter with serial number '{request.SerialNumber}' already exists.");
        }

        var scooter = Scooter.New(
            Guid.NewGuid(), request.Model, request.SerialNumber, request.BatteryPercent,
            request.PricePerMinute, request.IsAvailable, request.LastServiceDate);

        return await scooterRepository.Add(scooter, cancellationToken);
    }
}
