namespace Domain.Scooters;

public record ScooterId(Guid Value)
{
    public static ScooterId New() => new(Guid.NewGuid());
    public static ScooterId Empty() => new(Guid.Empty);
}