using Domain.Scooters;

namespace Api.Contracts;

public record ScooterResponse(
    Guid Id,
    string SerialNumber,
    int BatteryLevel,
    bool IsAvailable,
    DateTime LastMaintenanceDate,
    DateTime CreatedAt)
{
    public static ScooterResponse FromDomain(Scooter scooter) =>
        new(
            scooter.Id.Value,
            scooter.SerialNumber,
            scooter.BatteryLevel,
            scooter.IsAvailable,
            scooter.LastMaintenanceDate,
            scooter.CreatedAt
        );
}