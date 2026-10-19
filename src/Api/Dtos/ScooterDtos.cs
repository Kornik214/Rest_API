using Domain.Scooters;

namespace Api.Dtos;

public record ScooterDto(
    Guid Id,
    string Model,
    string SerialNumber,
    int BatteryPercent,
    decimal PricePerMinute,
    bool IsAvailable,
    DateTime LastServiceDate,
    DateTime CreatedAt)
{
    public static ScooterDto FromDomainModel(Scooter scooter) => new(
        scooter.Id,
        scooter.Model,
        scooter.SerialNumber,
        scooter.BatteryPercent,
        scooter.PricePerMinute,
        scooter.IsAvailable,
        scooter.LastServiceDate,
        scooter.CreatedAt);
}

public record CreateScooterDto(
    string Model,
    string SerialNumber,
    int BatteryPercent,
    decimal PricePerMinute,
    bool IsAvailable,
    DateTime LastServiceDate);

public record UpdateScooterDto(
    string Model,
    string SerialNumber,
    int BatteryPercent,
    decimal PricePerMinute,
    bool IsAvailable,
    DateTime LastServiceDate);
