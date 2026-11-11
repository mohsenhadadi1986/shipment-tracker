using ShipmentTracker.Domain.Enums;

namespace ShipmentTracker.Domain.Entities;

/// <summary>
/// Status / location history entry. Linked to a shipment and optionally a specific container + port.
/// </summary>
public class TrackingEvent
{
    public int Id { get; set; }

    public TrackingEventType EventType { get; set; }

    public required string Description { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public int ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = null!;

    public int? ContainerId { get; set; }
    public Container? Container { get; set; }

    public int? PortId { get; set; }
    public Port? Port { get; set; }
}
