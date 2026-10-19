using Application.Common.Interfaces.Repositories;
using MediatR;

namespace Application.Scooters.Commands;

public record DeleteScooterCommand : IRequest<bool>
{
    public required Guid ScooterId { get; init; }
}

public class DeleteScooterCommandHandler(IScooterRepository scooterRepository)
    : IRequestHandler<DeleteScooterCommand, bool>
{
    public async Task<bool> Handle(DeleteScooterCommand request, CancellationToken cancellationToken)
    {
        var scooter = await scooterRepository.GetById(request.ScooterId, cancellationToken);
        if (scooter is null)
        {
            return false;
        }

        await scooterRepository.Delete(scooter, cancellationToken);
        return true;
    }
}
