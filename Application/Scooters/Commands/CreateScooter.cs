using Domain.Scooters;
using FluentValidation;
using LanguageExt;
using MediatR;

namespace Application.Scooters.Commands;

public record CreateScooterCommand(string SerialNumber, int BatteryLevel, bool IsAvailable, DateTime LastMaintenanceDate) 
    : IRequest<Either<ScooterError, Scooter>>;

public class CreateScooterCommandValidator : AbstractValidator<CreateScooterCommand>
{
    public CreateScooterCommandValidator()
    {
        RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BatteryLevel).InclusiveBetween(0, 100);
        RuleFor(x => x.LastMaintenanceDate).LessThanOrEqualTo(DateTime.UtcNow);
    }
}

public class CreateScooterCommandHandler : IRequestHandler<CreateScooterCommand, Either<ScooterError, Scooter>>
{
    private readonly IScooterRepository _repository;

    public CreateScooterCommandHandler(IScooterRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<ScooterError, Scooter>> Handle(CreateScooterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _repository.GetBySerialNumberAsync(request.SerialNumber, cancellationToken);
            if (existing is not null)
            {
                return new ScooterAlreadyExistsError(request.SerialNumber);
            }

            var scooter = Scooter.Create(request.SerialNumber, request.BatteryLevel, request.IsAvailable, request.LastMaintenanceDate);
            await _repository.AddAsync(scooter, cancellationToken);
            
            return scooter;
        }
        catch (Exception ex)
        {
            return new ScooterUnhandledError(ex.Message);
        }
    }
}