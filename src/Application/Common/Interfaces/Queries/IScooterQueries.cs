using Domain.Scooters;

namespace Application.Common.Interfaces.Queries;

public interface IScooterQueries
{
    Task<IReadOnlyList<Scooter>> GetAll(CancellationToken cancellationToken);

    Task<Scooter?> GetById(Guid id, CancellationToken cancellationToken);
}
