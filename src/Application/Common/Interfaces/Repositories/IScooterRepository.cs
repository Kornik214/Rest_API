using Domain.Scooters;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface IScooterRepository
{
    Task<Option<Scooter>> GetById(ScooterId id, CancellationToken cancellationToken);

    Task<Option<Scooter>> GetBySerialNumber(string serialNumber, CancellationToken cancellationToken);

    Task<Scooter> Add(Scooter scooter, CancellationToken cancellationToken);

    Task<Scooter> Update(Scooter scooter, CancellationToken cancellationToken);

    Task Delete(Scooter scooter, CancellationToken cancellationToken);
}
