using Domain.Scooters;

namespace Tests.Data;

public static class ScootersTestData
{
    public static Scooter First() => Scooter.Create("KIV-ALPHA-001", 95, true, Utc(2026, 1, 10));
    public static Scooter Second() => Scooter.Create("KIV-BETA-002", 70, false, Utc(2026, 3, 15));
    public static Scooter Third() => Scooter.Create("LVIV-GAMMA-003", 40, true, Utc(2026, 6, 20));
    public static Scooter Fourth() => Scooter.Create("ODESA-DELTA-004", 15, false, Utc(2026, 9, 25));

    private static DateTime Utc(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Utc);
}
