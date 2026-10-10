using Application.Common.Interfaces;
using Domain.Scooters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ScooterRepository(ApplicationDbContext context) : IScooterRepository
{
    public async Task<IReadOnlyList<Scooter>> GetAll(
        CancellationToken cancellationToken)
    {
        return await context.Scooters.ToListAsync(cancellationToken);
    }

    public async Task<Scooter?> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await context.Scooters.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Scooter?> GetBySerialNumber(
        string serialNumber,
        CancellationToken cancellationToken)
    {
        return await context.Scooters.FirstOrDefaultAsync(
            x => x.SerialNumber.ToLower() == serialNumber.ToLower(),
            cancellationToken);
    }

    public async Task<Scooter> Add(
        Scooter scooter,
        CancellationToken cancellationToken)
    {
        await context.Scooters.AddAsync(scooter, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return scooter;
    }

    public async Task<Scooter> Update(
        Scooter scooter,
        CancellationToken cancellationToken)
    {
        context.Scooters.Update(scooter);
        await context.SaveChangesAsync(cancellationToken);
        return scooter;
    }

    public async Task Delete(
        Scooter scooter,
        CancellationToken cancellationToken)
    {
        context.Scooters.Remove(scooter);
        await context.SaveChangesAsync(cancellationToken);
    }
}
