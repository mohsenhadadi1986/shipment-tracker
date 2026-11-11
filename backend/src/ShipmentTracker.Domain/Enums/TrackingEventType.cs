namespace ShipmentTracker.Domain.Enums;

/// <summary>
/// Milestone / exception type recorded against a container or shipment.
/// </summary>
public enum TrackingEventType
{
    Booked = 0,
    Loaded = 1,
    Departed = 2,
    Arrived = 3,
    Discharged = 4,
    Delivered = 5,
    Exception = 6
}
