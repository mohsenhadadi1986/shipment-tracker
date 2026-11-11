using ShipmentTracker.Domain.Enums;

namespace ShipmentTracker.Domain.Entities;

/// <summary>
/// A booked movement of one or more containers from an origin port to a destination port.
/// </summary>
public class Shipment
{
    public int Id { get; set; }

    public required string ReferenceNumber { get; set; }

    public ShipmentStatus Status { get; set; }

    public int OriginPortId { get; set; }
    public Port OriginPort { get; set; } = null!;

    public int DestinationPortId { get; set; }
    public Port DestinationPort { get; set; } = null!;

    public int? VesselId { get; set; }
    public Vessel? Vessel { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DepartedAt { get; set; }

    public DateTimeOffset? ArrivedAt { get; set; }

    public ICollection<ShipmentContainer> ShipmentContainers { get; set; } = new List<ShipmentContainer>();
    public ICollection<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();
}
