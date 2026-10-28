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

    public async Task<List<Scooter>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Scooters.AsNoTracking().ToListAsync(cancellationToken);
    }

    // Явна реалізація методу з IScooterQueries, щоб повертати Option<Scooter>
    async Task<Option<Scooter>> IScooterQueries.GetByIdAsync(ScooterId id, CancellationToken cancellationToken)
    {
        var scooter = await _dbContext.Scooters.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        return scooter is null ? Option<Scooter>.None : Option<Scooter>.Some(scooter);
    }
}