using Domain.Scooters;

namespace Application.Scooters.Exceptions;

public abstract class ScooterException(
    ScooterId scooterId,
    string message,
    Exception? innerException = null) : Exception(message, innerException)
{
    public ScooterId ScooterId { get; } = scooterId;
}

public sealed class ScooterAlreadyExistsException(ScooterId scooterId)
    : ScooterException(scooterId, $"Scooter already exists under id {scooterId}");

public sealed class ScooterNotFoundException(ScooterId scooterId)
    : ScooterException(scooterId, $"Scooter not found under id {scooterId}");

public sealed class ScooterUnhandledException(ScooterId scooterId, Exception? innerException = null)
    : ScooterException(scooterId, $"Unhandled exception under id {scooterId}", innerException);
