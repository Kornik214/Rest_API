namespace Domain.Scooters;

public class Scooter
{
    public ScooterId Id { get; }

    public string Model { get; private set; }

    public string SerialNumber { get; private set; }

    public int BatteryPercent { get; private set; }

    public decimal PricePerMinute { get; private set; }

    public bool IsAvailable { get; private set; }

    public DateTime LastServiceDate { get; private set; }

    public DateTime CreatedAt { get; }

    private Scooter(
        ScooterId id,
        string model,
        string serialNumber,
        int batteryPercent,
        decimal pricePerMinute,
        bool isAvailable,
        DateTime lastServiceDate,
        DateTime createdAt)
    {
        Id = id;
        Model = model;
        SerialNumber = serialNumber;
        BatteryPercent = batteryPercent;
        PricePerMinute = pricePerMinute;
        IsAvailable = isAvailable;
        LastServiceDate = lastServiceDate;
        CreatedAt = createdAt;
    }

    public static Scooter New(
        ScooterId id,
        string model,
        string serialNumber,
        int batteryPercent,
        decimal pricePerMinute,
        bool isAvailable,
        DateTime lastServiceDate)
    {
        return new Scooter(
            id,
            model,
            serialNumber,
            batteryPercent,
            pricePerMinute,
            isAvailable,
            lastServiceDate,
            DateTime.UtcNow);
    }

    public void UpdateDetails(
        string model,
        string serialNumber,
        int batteryPercent,
        decimal pricePerMinute,
        bool isAvailable,
        DateTime lastServiceDate)
    {
        Model = model;
        SerialNumber = serialNumber;
        BatteryPercent = batteryPercent;
        PricePerMinute = pricePerMinute;
        IsAvailable = isAvailable;
        LastServiceDate = lastServiceDate;
    }
}
