using Domain.Scooters;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IScooterQueries
{
    Task<IReadOnlyList<Scooter>> GetAll(CancellationToken cancellationToken);

    Task<Option<Scooter>> GetById(ScooterId id, CancellationToken cancellationToken);
}
