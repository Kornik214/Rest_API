namespace Api.Contracts;

public record SearchScootersRequest(
    string? SerialNumber,
    int? BatteryLevelFrom,
    int? BatteryLevelTo,
    bool? IsAvailable,
    DateTime? LastMaintenanceDateFrom,
    DateTime? LastMaintenanceDateTo,
    DateTime? CreatedAtFrom,
    DateTime? CreatedAtTo);
