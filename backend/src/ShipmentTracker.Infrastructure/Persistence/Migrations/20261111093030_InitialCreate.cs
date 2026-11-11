using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShipmentTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnLocode = table.Column<string>(type: "nchar(5)", fixedLength: true, maxLength: 5, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vessels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImoNumber = table.Column<string>(type: "nchar(7)", fixedLength: true, maxLength: 7, nullable: false),
                    FlagCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CapacityTeu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vessels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContainerNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrentVesselId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Containers_Vessels_CurrentVesselId",
                        column: x => x.CurrentVesselId,
                        principalTable: "Vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OriginPortId = table.Column<int>(type: "int", nullable: false),
                    DestinationPortId = table.Column<int>(type: "int", nullable: false),
                    VesselId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DepartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ArrivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Ports_DestinationPortId",
                        column: x => x.DestinationPortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Ports_OriginPortId",
                        column: x => x.OriginPortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Vessels_VesselId",
                        column: x => x.VesselId,
                        principalTable: "Vessels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentContainers",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    ContainerId = table.Column<int>(type: "int", nullable: false),
                    SealNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentContainers", x => new { x.ShipmentId, x.ContainerId });
                    table.ForeignKey(
                        name: "FK_ShipmentContainers_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentContainers_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackingEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    ContainerId = table.Column<int>(type: "int", nullable: true),
                    PortId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackingEvents_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TrackingEvents_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TrackingEvents_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Containers",
                columns: new[] { "Id", "ContainerNumber", "CurrentVesselId", "Status", "Type" },
                values: new object[] { 3, "MSCU1112223", null, "Available", "Dry" });

            migrationBuilder.InsertData(
                table: "Ports",
                columns: new[] { "Id", "Country", "Latitude", "Longitude", "Name", "UnLocode" },
                values: new object[,]
                {
                    { 1, "Italy", 44.405600m, 8.946300m, "Genoa", "ITGOA" },
                    { 2, "Netherlands", 51.922500m, 4.479200m, "Rotterdam", "NLRTM" },
                    { 3, "China", 31.230400m, 121.473700m, "Shanghai", "CNSHA" },
                    { 4, "Singapore", 1.264400m, 103.822200m, "Singapore", "SGSIN" }
                });

            migrationBuilder.InsertData(
                table: "Vessels",
                columns: new[] { "Id", "CapacityTeu", "FlagCountry", "ImoNumber", "Name" },
                values: new object[,]
                {
                    { 1, 19224, "Panama", "9703291", "MSC Oscar" },
                    { 2, 23756, "Liberia", "9839430", "MSC Gülsün" }
                });

            migrationBuilder.InsertData(
                table: "Containers",
                columns: new[] { "Id", "ContainerNumber", "CurrentVesselId", "Status", "Type" },
                values: new object[,]
                {
                    { 1, "MSCU1234567", 1, "InTransit", "Dry" },
                    { 2, "MSCU7654321", 1, "Loaded", "Reefer" }
                });

            migrationBuilder.InsertData(
                table: "Shipments",
                columns: new[] { "Id", "ArrivedAt", "CreatedAt", "DepartedAt", "DestinationPortId", "OriginPortId", "ReferenceNumber", "Status", "VesselId" },
                values: new object[,]
                {
                    { 1, null, new DateTimeOffset(new DateTime(2026, 3, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 3, 5, 14, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 1, "SHP-2026-0001", "InTransit", 1 },
                    { 2, null, new DateTimeOffset(new DateTime(2026, 3, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 4, 2, "SHP-2026-0002", "Booked", 2 }
                });

            migrationBuilder.InsertData(
                table: "ShipmentContainers",
                columns: new[] { "ContainerId", "ShipmentId", "SealNumber" },
                values: new object[,]
                {
                    { 1, 1, "SEAL-A1001" },
                    { 2, 1, "SEAL-A1002" },
                    { 3, 2, "SEAL-B2001" }
                });

            migrationBuilder.InsertData(
                table: "TrackingEvents",
                columns: new[] { "Id", "ContainerId", "Description", "EventType", "OccurredAt", "PortId", "ShipmentId" },
                values: new object[,]
                {
                    { 1, null, "Shipment SHP-2026-0001 booked Genoa → Shanghai", "Booked", new DateTimeOffset(new DateTime(2026, 3, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 1 },
                    { 2, 1, "Container MSCU1234567 loaded onto MSC Oscar", "Loaded", new DateTimeOffset(new DateTime(2026, 3, 4, 16, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 1 },
                    { 3, null, "Vessel MSC Oscar departed Genoa", "Departed", new DateTimeOffset(new DateTime(2026, 3, 5, 14, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 1 },
                    { 4, null, "Shipment SHP-2026-0002 booked Rotterdam → Singapore", "Booked", new DateTimeOffset(new DateTime(2026, 3, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerNumber",
                table: "Containers",
                column: "ContainerNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Containers_CurrentVesselId",
                table: "Containers",
                column: "CurrentVesselId");

            migrationBuilder.CreateIndex(
                name: "IX_Ports_UnLocode",
                table: "Ports",
                column: "UnLocode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentContainers_ContainerId",
                table: "ShipmentContainers",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DestinationPortId",
                table: "Shipments",
                column: "DestinationPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OriginPortId",
                table: "Shipments",
                column: "OriginPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ReferenceNumber",
                table: "Shipments",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Status",
                table: "Shipments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_VesselId",
                table: "Shipments",
                column: "VesselId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingEvents_ContainerId",
                table: "TrackingEvents",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingEvents_OccurredAt",
                table: "TrackingEvents",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingEvents_PortId",
                table: "TrackingEvents",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingEvents_ShipmentId",
                table: "TrackingEvents",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_ImoNumber",
                table: "Vessels",
                column: "ImoNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentContainers");

            migrationBuilder.DropTable(
                name: "TrackingEvents");

            migrationBuilder.DropTable(
                name: "Containers");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "Ports");

            migrationBuilder.DropTable(
                name: "Vessels");
        }
    }
}
