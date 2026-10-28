namespace Api.Contracts;

public record CreateScooterRequest(
    string SerialNumber,
    int BatteryLevel,
    bool IsAvailable,
    DateTime LastMaintenanceDate);