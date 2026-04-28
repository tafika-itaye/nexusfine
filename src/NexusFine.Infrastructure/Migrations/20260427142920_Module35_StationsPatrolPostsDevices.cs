using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusFine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Module35_StationsPatrolPostsDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrimaryPatrolPostId",
                table: "Officers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StationId",
                table: "Officers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PhysicalAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OfficerInChargeBadge = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    StationServerEndpoint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StationServerPublicKey = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsecutiveFailedSyncs = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stations_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Serial = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Imei = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    CurrentOfficerId = table.Column<int>(type: "int", nullable: true),
                    AppVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PairedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Officers_CurrentOfficerId",
                        column: x => x.CurrentOfficerId,
                        principalTable: "Officers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Devices_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatrolPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatrolPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatrolPosts_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Code", "ConsecutiveFailedSyncs", "ContactPhone", "CreatedAt", "DepartmentId", "IsActive", "LastSyncAt", "Lat", "Lng", "Name", "OfficerInChargeBadge", "PhysicalAddress", "StationServerEndpoint", "StationServerPublicKey", "UpdatedAt", "Zone" },
                values: new object[,]
                {
                    { 1, "STN-LIL-001", 0, "+265 1 750 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, null, null, null, "Area 18 Police Station", null, "Area 18, Lilongwe", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lilongwe" },
                    { 2, "STN-LIL-002", 0, "+265 1 750 200", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, null, null, null, "Kamuzu Highway Checkpoint", null, "M1 Kamuzu Highway, North", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lilongwe" },
                    { 3, "STN-BLT-001", 0, "+265 1 840 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, null, null, null, "Limbe Police Station", null, "Limbe, Blantyre", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Blantyre" },
                    { 4, "STN-MZU-001", 0, "+265 1 332 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, true, null, null, null, "Mzuzu Central Police Station", null, "Mzuzu CBD", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mzuzu" },
                    { 5, "STN-ZBA-001", 0, "+265 1 525 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, true, null, null, null, "Zomba Police Station", null, "Zomba CBD", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Zomba" }
                });

            migrationBuilder.InsertData(
                table: "PatrolPosts",
                columns: new[] { "Id", "Code", "CreatedAt", "IsActive", "Lat", "Lng", "Name", "Notes", "StationId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "PP-LIL-018-A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, "Kamuzu Hwy North", null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "PP-LIL-018-B", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, "Kamuzu Hwy South", null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "PP-LIL-001-A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, "Area 18 Roundabout", null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "PP-BLT-001-A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, "Limbe Market Junction", null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "PP-MZU-001-A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, "Mzuzu Bus Depot", null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "PP-ZBA-001-A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, "Zomba M3 Junction", null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Officers_PrimaryPatrolPostId",
                table: "Officers",
                column: "PrimaryPatrolPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Officers_StationId",
                table: "Officers",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CurrentOfficerId",
                table: "Devices",
                column: "CurrentOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Serial",
                table: "Devices",
                column: "Serial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_StationId",
                table: "Devices",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Status",
                table: "Devices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PatrolPosts_Code",
                table: "PatrolPosts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatrolPosts_StationId",
                table: "PatrolPosts",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_Code",
                table: "Stations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_DepartmentId",
                table: "Stations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_Zone",
                table: "Stations",
                column: "Zone");

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_PatrolPosts_PrimaryPatrolPostId",
                table: "Officers",
                column: "PrimaryPatrolPostId",
                principalTable: "PatrolPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Officers_Stations_StationId",
                table: "Officers",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Officers_PatrolPosts_PrimaryPatrolPostId",
                table: "Officers");

            migrationBuilder.DropForeignKey(
                name: "FK_Officers_Stations_StationId",
                table: "Officers");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "PatrolPosts");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Officers_PrimaryPatrolPostId",
                table: "Officers");

            migrationBuilder.DropIndex(
                name: "IX_Officers_StationId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "PrimaryPatrolPostId",
                table: "Officers");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Officers");
        }
    }
}
