using Domain.Scooters;
using LanguageExt;

namespace Application.Scooters;

public interface IScooterQueries
{
    Task<IReadOnlyList<Scooter>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Option<Scooter>> GetByIdAsync(ScooterId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Scooter>> FilterAsync(
        string? serialNumber,
        int? batteryLevelFrom,
        int? batteryLevelTo,
        bool? isAvailable,
        DateTime? lastMaintenanceDateFrom,
        DateTime? lastMaintenanceDateTo,
        DateTime? createdAtFrom,
        DateTime? createdAtTo,
        CancellationToken cancellationToken = default);
}