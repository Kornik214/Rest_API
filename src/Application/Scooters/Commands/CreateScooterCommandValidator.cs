using FluentValidation;

namespace Application.Scooters.Commands;

public class CreateScooterCommandValidator : AbstractValidator<CreateScooterCommand>
{
    public CreateScooterCommandValidator()
    {
        RuleFor(x => x.Model).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.SerialNumber).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.BatteryPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.PricePerMinute).InclusiveBetween(0.01m, 1000m);
        RuleFor(x => x.IsAvailable).NotNull();
        RuleFor(x => x.LastServiceDate).NotEmpty();
    }
}
