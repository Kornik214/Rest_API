using Domain.Scooters;

namespace Application.Common.Interfaces.Repositories;

public interface IScooterRepository
{
    Task<Scooter?> GetById(Guid id, CancellationToken cancellationToken);

    Task<Scooter?> GetBySerialNumber(string serialNumber, CancellationToken cancellationToken);

    Task<Scooter> Add(Scooter scooter, CancellationToken cancellationToken);

    Task<Scooter> Update(Scooter scooter, CancellationToken cancellationToken);

    Task Delete(Scooter scooter, CancellationToken cancellationToken);
}
