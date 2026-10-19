namespace Domain.Scooters;

public record ScooterId(Guid Value)
{
    public static ScooterId Empty() => new(Guid.Empty);

    public static ScooterId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
