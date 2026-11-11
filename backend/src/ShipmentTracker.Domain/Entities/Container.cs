using ShipmentTracker.Domain.Enums;

namespace ShipmentTracker.Domain.Entities;

/// <summary>
/// Physical shipping container (e.g. MSCU1234567).
/// Can belong to many shipments over its life (many-to-many via ShipmentContainer).
/// </summary>
public class Container
{
    public int Id { get; set; }

    /// <summary>BIC container number, typically 11 characters (4 letters + 7 digits).</summary>
    public required string ContainerNumber { get; set; }

    public ContainerType Type { get; set; }

    public ContainerStatus Status { get; set; }

    /// <summary>Vessel this container is currently on, if any.</summary>
    public int? CurrentVesselId { get; set; }
    public Vessel? CurrentVessel { get; set; }

    public ICollection<ShipmentContainer> ShipmentContainers { get; set; } = new List<ShipmentContainer>();
    public ICollection<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();
}
