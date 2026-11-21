using Application.Scooters;
using Domain.Scooters;
using Infrastructure.Database;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScooterRepository : IScooterRepository, IScooterQueries
{
    private readonly ApplicationDbContext _dbContext;

    public ScooterRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Scooter?> GetByIdAsync(ScooterId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Scooters.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Scooter?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Scooters.FirstOrDefaultAsync(s => s.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task AddAsync(Scooter scooter, CancellationToken cancellationToken = default)
    {
        await _dbContext.Scooters.AddAsync(scooter, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Scooter scooter, CancellationToken cancellationToken = default)
    {
        _dbContext.Scooters.Update(scooter);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Scooter>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Scooters.AsNoTracking().ToListAsync(cancellationToken);
    }
    async Task<Option<Scooter>> IScooterQueries.GetByIdAsync(ScooterId id, CancellationToken cancellationToken)
    {
        var scooter = await _dbContext.Scooters.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        return scooter is null ? Option<Scooter>.None : Option<Scooter>.Some(scooter);
    }

    public async Task<IReadOnlyList<Scooter>> FilterAsync(
        string? serialNumber,
        int? batteryLevelFrom,
        int? batteryLevelTo,
        bool? isAvailable,
        DateTime? lastMaintenanceDateFrom,
        DateTime? lastMaintenanceDateTo,
        DateTime? createdAtFrom,
        DateTime? createdAtTo,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Scooter> query = _dbContext.Scooters.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(serialNumber))
        {
            var search = serialNumber.Trim().ToLower();
            query = query.Where(s => s.SerialNumber.ToLower().Contains(search));
        }

        if (batteryLevelFrom.HasValue)
            query = query.Where(s => s.BatteryLevel >= batteryLevelFrom.Value);
        if (batteryLevelTo.HasValue)
            query = query.Where(s => s.BatteryLevel <= batteryLevelTo.Value);
        if (isAvailable.HasValue)
            query = query.Where(s => s.IsAvailable == isAvailable.Value);
        if (lastMaintenanceDateFrom.HasValue)
            query = query.Where(s => s.LastMaintenanceDate >= DateTime.SpecifyKind(lastMaintenanceDateFrom.Value, DateTimeKind.Utc));
        if (lastMaintenanceDateTo.HasValue)
            query = query.Where(s => s.LastMaintenanceDate <= DateTime.SpecifyKind(lastMaintenanceDateTo.Value, DateTimeKind.Utc));
        if (createdAtFrom.HasValue)
            query = query.Where(s => s.CreatedAt >= DateTime.SpecifyKind(createdAtFrom.Value, DateTimeKind.Utc));
        if (createdAtTo.HasValue)
            query = query.Where(s => s.CreatedAt <= DateTime.SpecifyKind(createdAtTo.Value, DateTimeKind.Utc));

        return await query.ToListAsync(cancellationToken);
    }
}
