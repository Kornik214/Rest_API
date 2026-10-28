using Domain.Scooters;

namespace Application.Scooters;

public interface IScooterRepository
{
    Task<Scooter?> GetByIdAsync(ScooterId id, CancellationToken cancellationToken = default);
    Task<Scooter?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Scooter scooter, CancellationToken cancellationToken = default);
    Task UpdateAsync(Scooter scooter, CancellationToken cancellationToken = default);
}