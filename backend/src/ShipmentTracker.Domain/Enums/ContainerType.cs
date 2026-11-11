namespace ShipmentTracker.Domain.Enums;

/// <summary>
/// Physical / commercial type of a shipping container (ISO family simplified for learning).
/// Stored as int in SQL Server via EF Core by default.
/// </summary>
public enum ContainerType
{
    Dry = 0,
    Reefer = 1,
    OpenTop = 2,
    FlatRack = 3,
    Tank = 4
}
