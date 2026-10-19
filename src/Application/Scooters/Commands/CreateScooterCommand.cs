using Application.Common.Interfaces.Repositories;
using Application.Scooters.Exceptions;
using Domain.Scooters;
using LanguageExt;
using MediatR;

namespace Application.Scooters.Commands;

public record CreateScooterCommand : IRequest<Either<ScooterException, Scooter>>
{
    public required string Model { get; init; }
    public required string SerialNumber { get; init; }
    public required int BatteryPercent { get; init; }
    public required decimal PricePerMinute { get; init; }
    public required bool IsAvailable { get; init; }
    public required DateTime LastServiceDate { get; init; }
}

public class CreateScooterCommandHandler(IScooterRepository scooterRepository)
    : IRequestHandler<CreateScooterCommand, Either<ScooterException, Scooter>>
{
    public async Task<Either<ScooterException, Scooter>> Handle(
        CreateScooterCommand request,
        CancellationToken cancellationToken)
    {
        var existingScooter = await scooterRepository.GetBySerialNumber(
            request.SerialNumber, cancellationToken);

        return await existingScooter.MatchAsync(
            scooter => new ScooterAlreadyExistsException(scooter.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<ScooterException, Scooter>> CreateEntity(
        CreateScooterCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var scooter = Scooter.New(
                ScooterId.New(), request.Model, request.SerialNumber, request.BatteryPercent,
                request.PricePerMinute, request.IsAvailable, request.LastServiceDate);

            return await scooterRepository.Add(scooter, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ScooterUnhandledException(ScooterId.Empty(), exception);
        }
    }
}
