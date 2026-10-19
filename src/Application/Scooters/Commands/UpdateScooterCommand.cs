using Application.Common.Interfaces.Repositories;
using Application.Scooters.Exceptions;
using Domain.Scooters;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Scooters.Commands;

public record UpdateScooterCommand : IRequest<Either<ScooterException, Scooter>>
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
    : IRequestHandler<UpdateScooterCommand, Either<ScooterException, Scooter>>
{
    public async Task<Either<ScooterException, Scooter>> Handle(
        UpdateScooterCommand request,
        CancellationToken cancellationToken)
    {
        var scooterId = new ScooterId(request.ScooterId);
        var scooter = await scooterRepository.GetById(scooterId, cancellationToken);

        return await scooter.MatchAsync(
            found => CheckDuplicates(found.Id, request.SerialNumber, cancellationToken)
                .BindAsync(_ => UpdateEntity(found, request, cancellationToken)),
            () => new ScooterNotFoundException(scooterId));
    }

    private async Task<Either<ScooterException, Unit>> CheckDuplicates(
        ScooterId currentScooterId,
        string serialNumber,
        CancellationToken cancellationToken)
    {
        var scooter = await scooterRepository.GetBySerialNumber(serialNumber, cancellationToken);

        return scooter.Match<Either<ScooterException, Unit>>(
            found => found.Id == currentScooterId
                ? Unit.Default
                : new ScooterAlreadyExistsException(found.Id),
            () => Unit.Default);
    }

    private async Task<Either<ScooterException, Scooter>> UpdateEntity(
        Scooter scooter,
        UpdateScooterCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            scooter.UpdateDetails(request.Model, request.SerialNumber, request.BatteryPercent,
                request.PricePerMinute, request.IsAvailable, request.LastServiceDate);

            return await scooterRepository.Update(scooter, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ScooterUnhandledException(scooter.Id, exception);
        }
    }
}
