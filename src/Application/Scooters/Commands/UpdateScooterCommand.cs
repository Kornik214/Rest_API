using Application.Common.Interfaces.Repositories;
using Domain.Scooters;
using MediatR;

namespace Application.Scooters.Commands;

public record UpdateScooterCommand : IRequest<Scooter?>
{
    public required Guid ScooterId { get; init; }
    public required string Model { get; init; }
    public required string SerialNumber { get; init; }
    public required int BatteryPercent { get; init; }
    public required decimal PricePerMinute { get; init; }
    public required bool IsAvailable { get; init; }
    public required DateTime LastServiceDate { get; init; }
}

public class UpdateScooterCommandHandler(IScooterRepository scooterRepository)
    : IRequestHandler<UpdateScooterCommand, Scooter?>
{
    public async Task<Scooter?> Handle(UpdateScooterCommand request, CancellationToken cancellationToken)
    {
        var scooter = await scooterRepository.GetById(request.ScooterId, cancellationToken);
        if (scooter is null)
        {
            return null;
        }

        var duplicate = await scooterRepository.GetBySerialNumber(
            request.SerialNumber, cancellationToken);
        if (duplicate is not null && duplicate.Id != request.ScooterId)
        {
            throw new ArgumentException(
                $"Scooter with serial number '{request.SerialNumber}' already exists.");
        }

        scooter.UpdateDetails(request.Model, request.SerialNumber, request.BatteryPercent,
            request.PricePerMinute, request.IsAvailable, request.LastServiceDate);

        return await scooterRepository.Update(scooter, cancellationToken);
    }
}
