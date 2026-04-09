using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusFine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    HeadOfficerBadge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OffenceCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DefaultFineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffenceCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Officers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BadgeNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NfcTagId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DeviceImei = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLatitude = table.Column<double>(type: "float", nullable: true),
                    LastLongitude = table.Column<double>(type: "float", nullable: true),
                    LastKnownLocation = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Officers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Officers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    VehicleMake = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleColour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DriverNationalId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DriverLicenceNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DriverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OffenceCodeId = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvidencePhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OfficerId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fines_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fines_OffenceCodes_OffenceCodeId",
                        column: x => x.OffenceCodeId,
                        principalTable: "OffenceCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fines_Officers_OfficerId",
                        column: x => x.OfficerId,
                        principalTable: "Officers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FineId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewerNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByOfficerId = table.Column<int>(type: "int", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appeals_Fines_FineId",
                        column: x => x.FineId,
                        principalTable: "Fines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    FineId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Fines_FineId",
                        column: x => x.FineId,
                        principalTable: "Fines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "HeadOfficerBadge", "IsActive", "Name", "Zone" },
                values: new object[,]
                {
                    { 1, null, true, "Lilongwe Traffic", "Lilongwe" },
                    { 2, null, true, "Blantyre Traffic", "Blantyre" },
                    { 3, null, true, "Mzuzu Traffic", "Mzuzu" },
                    { 4, null, true, "Zomba Traffic", "Zomba" }
                });

            migrationBuilder.InsertData(
                table: "OffenceCodes",
                columns: new[] { "Id", "Code", "DefaultFineAmount", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "OC-001", 50000m, "Exceeding the posted speed limit", true, "Speeding" },
                    { 2, "OC-002", 75000m, "Failure to stop at a red traffic light", true, "Red Light Violation" },
                    { 3, "OC-003", 25000m, "Driver or passenger not wearing seatbelt", true, "No Seatbelt" },
                    { 4, "OC-004", 40000m, "Driving with an expired road licence", true, "Expired Road Licence" },
                    { 5, "OC-005", 60000m, "Operating a vehicle in an unroadworthy condition", true, "Unroadworthy Vehicle" },
                    { 6, "OC-006", 30000m, "Driving in the wrong lane", true, "Wrong Lane" },
                    { 7, "OC-007", 20000m, "Motorcycle rider without a helmet", true, "No Helmet" },
                    { 8, "OC-008", 80000m, "Vehicle carrying load exceeding permitted weight", true, "Overloading" },
                    { 9, "OC-009", 100000m, "Operating a vehicle without a valid licence", true, "Driving Without Licence" },
                    { 10, "OC-010", 50000m, "Using a handheld mobile phone while driving", true, "Use of Mobile Phone While Driving" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appeals_FineId",
                table: "Appeals",
                column: "FineId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appeals_ReferenceNumber",
                table: "Appeals",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_DepartmentId",
                table: "Fines",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_DriverNationalId",
                table: "Fines",
                column: "DriverNationalId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_IssuedAt",
                table: "Fines",
                column: "IssuedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_OffenceCodeId",
                table: "Fines",
                column: "OffenceCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_OfficerId",
                table: "Fines",
                column: "OfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_PlateNumber",
                table: "Fines",
                column: "PlateNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Fines_ReferenceNumber",
                table: "Fines",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fines_Status",
                table: "Fines",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OffenceCodes_Code",
                table: "OffenceCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Officers_BadgeNumber",
                table: "Officers",
                column: "BadgeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DepartmentId",
                table: "Officers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Officers_DeviceId",
                table: "Officers",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Officers_NfcTagId",
                table: "Officers",
                column: "NfcTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FineId",
                table: "Payments",
                column: "FineId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReceiptNumber",
                table: "Payments",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionReference",
                table: "Payments",
                column: "TransactionReference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appeals");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Fines");

            migrationBuilder.DropTable(
                name: "OffenceCodes");

            migrationBuilder.DropTable(
                name: "Officers");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
