using Domain.Scooters;
using LanguageExt;

namespace Application.Scooters;

public interface IScooterQueries
{
    Task<List<Scooter>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Option<Scooter>> GetByIdAsync(ScooterId id, CancellationToken cancellationToken = default);
}