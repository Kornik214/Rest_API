using Domain.Scooters;

namespace Application.Scooters.Services.Abstract;

public interface IScooterService
{
    Task<IReadOnlyList<Scooter>> GetScooters(
        CancellationToken cancellationToken);

    Task<Scooter?> GetScooter(
        Guid id,
        CancellationToken cancellationToken);

    Task<Scooter> Add(
        string model,
        string serialNumber,
        int batteryPercent,
        decimal pricePerMinute,
        bool isAvailable,
        DateTime lastServiceDate,
        CancellationToken cancellationToken);

    Task<Scooter?> Update(
        Guid id,
        string model,
        string serialNumber,
        int batteryPercent,
        decimal pricePerMinute,
        bool isAvailable,
        DateTime lastServiceDate,
        CancellationToken cancellationToken);

    Task<bool> Delete(
        Guid id,
        CancellationToken cancellationToken);
}