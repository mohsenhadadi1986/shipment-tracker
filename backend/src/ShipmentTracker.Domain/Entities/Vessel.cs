namespace ShipmentTracker.Domain.Entities;

/// <summary>
/// Ocean-going ship that carries containers. Identified by IMO number in the real world.
/// </summary>
public class Vessel
{
    public int Id { get; set; }

    public required string Name { get; set; }

    /// <summary>International Maritime Organization number (7 digits as string for leading zeros).</summary>
    public required string ImoNumber { get; set; }

    public required string FlagCountry { get; set; }

    /// <summary>Nominal capacity in twenty-foot equivalent units (TEU).</summary>
    public int CapacityTeu { get; set; }

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<Container> Containers { get; set; } = new List<Container>();
}
