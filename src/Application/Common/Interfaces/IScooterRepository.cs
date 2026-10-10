using Domain.Scooters;

namespace Application.Common.Interfaces;

public interface IScooterRepository
{
    Task<IReadOnlyList<Scooter>> GetAll(CancellationToken cancellationToken);

    Task<Scooter?> GetById(Guid id, CancellationToken cancellationToken);

    Task<Scooter?> GetBySerialNumber(
        string serialNumber,
        CancellationToken cancellationToken);

    Task<Scooter> Add(
        Scooter scooter,
        CancellationToken cancellationToken);

    Task<Scooter> Update(
        Scooter scooter,
        CancellationToken cancellationToken);

    Task Delete(
        Scooter scooter,
        CancellationToken cancellationToken);
}