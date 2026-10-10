using System.ComponentModel.DataAnnotations;
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
    public static ScooterDto FromDomainModel(Scooter scooter)
    {
        return new ScooterDto(
            scooter.Id,
            scooter.Model,
            scooter.SerialNumber,
            scooter.BatteryPercent,
            scooter.PricePerMinute,
            scooter.IsAvailable,
            scooter.LastServiceDate,
            scooter.CreatedAt);
    }
}

public record CreateScooterDto(
    [Required, MaxLength(100)] string Model,
    [Required, MaxLength(50)] string SerialNumber,
    [Range(0, 100)] int BatteryPercent,
    [Range(0.01, 1000)] decimal PricePerMinute,
    bool IsAvailable,
    DateTime LastServiceDate);

public record UpdateScooterDto(
    [Required, MaxLength(100)] string Model,
    [Required, MaxLength(50)] string SerialNumber,
    [Range(0, 100)] int BatteryPercent,
    [Range(0.01, 1000)] decimal PricePerMinute,
    bool IsAvailable,
    DateTime LastServiceDate);