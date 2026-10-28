using Domain.Scooters;
using FluentValidation;
using LanguageExt;
using MediatR;

namespace Application.Scooters.Commands;

public record UpdateScooterCommand(Guid Id, string SerialNumber, int BatteryLevel, bool IsAvailable, DateTime LastMaintenanceDate) 
    : IRequest<Either<ScooterError, Scooter>>;

public class UpdateScooterCommandValidator : AbstractValidator<UpdateScooterCommand>
{
    public UpdateScooterCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BatteryLevel).InclusiveBetween(0, 100);
        RuleFor(x => x.LastMaintenanceDate).LessThanOrEqualTo(DateTime.UtcNow);
    }
}

public class UpdateScooterCommandHandler : IRequestHandler<UpdateScooterCommand, Either<ScooterError, Scooter>>
{
    private readonly IScooterRepository _repository;

    public UpdateScooterCommandHandler(IScooterRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<ScooterError, Scooter>> Handle(UpdateScooterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var scooterId = new ScooterId(request.Id);
            var scooter = await _repository.GetByIdAsync(scooterId, cancellationToken);
            
            if (scooter is null)
            {
                return new ScooterNotFoundError(scooterId);
            }

            var existingWithSerial = await _repository.GetBySerialNumberAsync(request.SerialNumber, cancellationToken);
            if (existingWithSerial is not null && existingWithSerial.Id != scooter.Id) // Збіг із самим собою не є помилкою
            {
                return new ScooterAlreadyExistsError(request.SerialNumber);
            }

            scooter.Update(request.SerialNumber, request.BatteryLevel, request.IsAvailable, request.LastMaintenanceDate);
            await _repository.UpdateAsync(scooter, cancellationToken);
            
            return scooter;
        }
        catch (Exception ex)
        {
            return new ScooterUnhandledError(ex.Message);
        }
    }
}