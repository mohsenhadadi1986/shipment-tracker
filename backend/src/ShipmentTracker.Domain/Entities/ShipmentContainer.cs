namespace ShipmentTracker.Domain.Entities;

/// <summary>
/// Join entity for the many-to-many between Shipment and Container.
/// Explicit join table (instead of a bare skip-navigation) so we can add payload later
/// (e.g. load sequence, seal number) without another migration redesign.
/// </summary>
public class ShipmentContainer
{
    public int ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = null!;

    public int ContainerId { get; set; }
    public Container Container { get; set; } = null!;

    /// <summary>Optional seal number recorded when the container was stuffed for this shipment.</summary>
    public string? SealNumber { get; set; }
}
