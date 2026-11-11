using ShipmentTracker.Domain.Entities;
using ShipmentTracker.Domain.Enums;

namespace ShipmentTracker.Infrastructure.Persistence;

/// <summary>
/// Deterministic demo rows applied via <c>HasData</c> in the initial migration.
/// Fixed integer IDs are required so EF can wire FKs in the migration snapshot.
/// </summary>
internal static class SeedData
{
    // Ports
    public const int GenoaId = 1;
    public const int RotterdamId = 2;
    public const int ShanghaiId = 3;
    public const int SingaporeId = 4;

    // Vessels
    public const int MscOscarId = 1;
    public const int MscGulsunId = 2;

    // Containers
    public const int ContainerDry1Id = 1;
    public const int ContainerReefer1Id = 2;
    public const int ContainerDry2Id = 3;

    // Shipments
    public const int ShipmentGenoaShanghaiId = 1;
    public const int ShipmentRotterdamSingaporeId = 2;

    public static Port[] Ports { get; } =
    [
        new Port
        {
            Id = GenoaId,
            Name = "Genoa",
            Country = "Italy",
            UnLocode = "ITGOA",
            Latitude = 44.405600m,
            Longitude = 8.946300m
        },
        new Port
        {
            Id = RotterdamId,
            Name = "Rotterdam",
            Country = "Netherlands",
            UnLocode = "NLRTM",
            Latitude = 51.922500m,
            Longitude = 4.479200m
        },
        new Port
        {
            Id = ShanghaiId,
            Name = "Shanghai",
            Country = "China",
            UnLocode = "CNSHA",
            Latitude = 31.230400m,
            Longitude = 121.473700m
        },
        new Port
        {
            Id = SingaporeId,
            Name = "Singapore",
            Country = "Singapore",
            UnLocode = "SGSIN",
            Latitude = 1.264400m,
            Longitude = 103.822200m
        }
    ];

    public static Vessel[] Vessels { get; } =
    [
        new Vessel
        {
            Id = MscOscarId,
            Name = "MSC Oscar",
            ImoNumber = "9703291",
            FlagCountry = "Panama",
            CapacityTeu = 19224
        },
        new Vessel
        {
            Id = MscGulsunId,
            Name = "MSC Gülsün",
            ImoNumber = "9839430",
            FlagCountry = "Liberia",
            CapacityTeu = 23756
        }
    ];

    public static Container[] Containers { get; } =
    [
        new Container
        {
            Id = ContainerDry1Id,
            ContainerNumber = "MSCU1234567",
            Type = ContainerType.Dry,
            Status = ContainerStatus.InTransit,
            CurrentVesselId = MscOscarId
        },
        new Container
        {
            Id = ContainerReefer1Id,
            ContainerNumber = "MSCU7654321",
            Type = ContainerType.Reefer,
            Status = ContainerStatus.Loaded,
            CurrentVesselId = MscOscarId
        },
        new Container
        {
            Id = ContainerDry2Id,
            ContainerNumber = "MSCU1112223",
            Type = ContainerType.Dry,
            Status = ContainerStatus.Available,
            CurrentVesselId = null
        }
    ];

    public static Shipment[] Shipments { get; } =
    [
        new Shipment
        {
            Id = ShipmentGenoaShanghaiId,
            ReferenceNumber = "SHP-2026-0001",
            Status = ShipmentStatus.InTransit,
            OriginPortId = GenoaId,
            DestinationPortId = ShanghaiId,
            VesselId = MscOscarId,
            CreatedAt = new DateTimeOffset(2026, 3, 1, 8, 0, 0, TimeSpan.Zero),
            DepartedAt = new DateTimeOffset(2026, 3, 5, 14, 30, 0, TimeSpan.Zero),
            ArrivedAt = null
        },
        new Shipment
        {
            Id = ShipmentRotterdamSingaporeId,
            ReferenceNumber = "SHP-2026-0002",
            Status = ShipmentStatus.Booked,
            OriginPortId = RotterdamId,
            DestinationPortId = SingaporeId,
            VesselId = MscGulsunId,
            CreatedAt = new DateTimeOffset(2026, 3, 10, 9, 0, 0, TimeSpan.Zero),
            DepartedAt = null,
            ArrivedAt = null
        }
    ];

    public static ShipmentContainer[] ShipmentContainers { get; } =
    [
        new ShipmentContainer
        {
            ShipmentId = ShipmentGenoaShanghaiId,
            ContainerId = ContainerDry1Id,
            SealNumber = "SEAL-A1001"
        },
        new ShipmentContainer
        {
            ShipmentId = ShipmentGenoaShanghaiId,
            ContainerId = ContainerReefer1Id,
            SealNumber = "SEAL-A1002"
        },
        new ShipmentContainer
        {
            ShipmentId = ShipmentRotterdamSingaporeId,
            ContainerId = ContainerDry2Id,
            SealNumber = "SEAL-B2001"
        }
    ];

    public static TrackingEvent[] TrackingEvents { get; } =
    [
        new TrackingEvent
        {
            Id = 1,
            EventType = TrackingEventType.Booked,
            Description = "Shipment SHP-2026-0001 booked Genoa → Shanghai",
            OccurredAt = new DateTimeOffset(2026, 3, 1, 8, 0, 0, TimeSpan.Zero),
            ShipmentId = ShipmentGenoaShanghaiId,
            ContainerId = null,
            PortId = GenoaId
        },
        new TrackingEvent
        {
            Id = 2,
            EventType = TrackingEventType.Loaded,
            Description = "Container MSCU1234567 loaded onto MSC Oscar",
            OccurredAt = new DateTimeOffset(2026, 3, 4, 16, 0, 0, TimeSpan.Zero),
            ShipmentId = ShipmentGenoaShanghaiId,
            ContainerId = ContainerDry1Id,
            PortId = GenoaId
        },
        new TrackingEvent
        {
            Id = 3,
            EventType = TrackingEventType.Departed,
            Description = "Vessel MSC Oscar departed Genoa",
            OccurredAt = new DateTimeOffset(2026, 3, 5, 14, 30, 0, TimeSpan.Zero),
            ShipmentId = ShipmentGenoaShanghaiId,
            ContainerId = null,
            PortId = GenoaId
        },
        new TrackingEvent
        {
            Id = 4,
            EventType = TrackingEventType.Booked,
            Description = "Shipment SHP-2026-0002 booked Rotterdam → Singapore",
            OccurredAt = new DateTimeOffset(2026, 3, 10, 9, 0, 0, TimeSpan.Zero),
            ShipmentId = ShipmentRotterdamSingaporeId,
            ContainerId = null,
            PortId = RotterdamId
        }
    ];
}
