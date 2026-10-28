namespace Domain.Scooters;

public class Scooter
{
    private Scooter(ScooterId id, string serialNumber, int batteryLevel, bool isAvailable, DateTime lastMaintenanceDate, DateTime createdAt)
    {
        Id = id;
        SerialNumber = serialNumber;
        BatteryLevel = batteryLevel;
        IsAvailable = isAvailable;
        LastMaintenanceDate = lastMaintenanceDate;
        CreatedAt = createdAt;
    }

    public ScooterId Id { get; private set; }
    public string SerialNumber { get; private set; } 
    public int BatteryLevel { get; private set; }     
    public bool IsAvailable { get; private set; }     
    public DateTime LastMaintenanceDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Scooter Create(string serialNumber, int batteryLevel, bool isAvailable, DateTime lastMaintenanceDate)
    {
        return new Scooter(
            ScooterId.New(),
            serialNumber,
            batteryLevel,
            isAvailable,
            lastMaintenanceDate,
            DateTime.UtcNow);
    }

    public void Update(string serialNumber, int batteryLevel, bool isAvailable, DateTime lastMaintenanceDate)
    {
        SerialNumber = serialNumber;
        BatteryLevel = batteryLevel;
        IsAvailable = isAvailable;
        LastMaintenanceDate = lastMaintenanceDate;
    }
}