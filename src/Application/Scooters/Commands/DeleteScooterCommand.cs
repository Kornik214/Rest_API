using Application.Common.Interfaces.Repositories;
using Application.Scooters.Exceptions;
using Domain.Scooters;
using LanguageExt;
using MediatR;

namespace Application.Scooters.Commands;

public record DeleteScooterCommand : IRequest<Either<ScooterException, Scooter>>
{
    public required Guid ScooterId { get; init; }
}

public class DeleteScooterCommandHandler(IScooterRepository scooterRepository)
    : IRequestHandler<DeleteScooterCommand, Either<ScooterException, Scooter>>
{
    public async Task<Either<ScooterException, Scooter>> Handle(
        DeleteScooterCommand request,
        CancellationToken cancellationToken)
    {
        var scooterId = new ScooterId(request.ScooterId);
        var scooter = await scooterRepository.GetById(scooterId, cancellationToken);

        return await scooter.MatchAsync(
            found => DeleteEntity(found, cancellationToken),
            () => new ScooterNotFoundException(scooterId));
    }

    private async Task<Either<ScooterException, Scooter>> DeleteEntity(
        Scooter scooter,
        CancellationToken cancellationToken)
    {
        try
        {
            await scooterRepository.Delete(scooter, cancellationToken);
            return scooter;
        }
        catch (Exception exception)
        {
            return new ScooterUnhandledException(scooter.Id, exception);
        }
    }
}
