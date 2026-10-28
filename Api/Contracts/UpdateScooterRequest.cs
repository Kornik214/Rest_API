namespace Api.Contracts;

public record UpdateScooterRequest(
    string SerialNumber,
    int BatteryLevel,
    bool IsAvailable,
    DateTime LastMaintenanceDate);