using Domain.Scooters;

namespace Tests.Data.Scooters;

public static class ScootersTestData
{
    public static Scooter FirstTestScooter() => Scooter.New(
        ScooterId.New(), "Ninebot E2", "TEST-SCOOTER-001", 85, 12.50m, true,
        new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc));

    public static Scooter SecondTestScooter() => Scooter.New(
        ScooterId.New(), "Xiaomi Pro 2", "TEST-SCOOTER-002", 70, 15.00m, true,
        new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));

    public static Scooter ThirdTestScooter() => Scooter.New(
        ScooterId.New(), "Segway Max G2", "TEST-SCOOTER-003", 95, 18.75m, false,
        new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc));
}
