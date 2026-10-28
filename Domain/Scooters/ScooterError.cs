namespace Domain.Scooters;

public abstract record ScooterError(string Message);

public record ScooterAlreadyExistsError(string SerialNumber) 
    : ScooterError($"Самокат із серійним номером '{SerialNumber}' вже існує.");

public record ScooterNotFoundError(ScooterId Id) 
    : ScooterError($"Самокат з ідентифікатором {Id.Value} не знайдено.");

public record ScooterUnhandledError(string Details) 
    : ScooterError($"Необроблена помилка: {Details}");