namespace ShipmentTracker.Domain.Entities;

/// <summary>
/// A seaport identified by a UN/LOCODE (e.g. ITGOA for Genoa).
/// </summary>
public class Port
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Country { get; set; }

    /// <summary>UN Location Code, 5 characters (e.g. NLRTM).</summary>
    public required string UnLocode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    // Navigation: shipments that start / end here
    public ICollection<Shipment> OriginShipments { get; set; } = new List<Shipment>();
    public ICollection<Shipment> DestinationShipments { get; set; } = new List<Shipment>();
    public ICollection<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();
}
