using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusFine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Module35b_DepartmentRegionAndFullDistricts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HeadOfficerBadge",
                table: "Departments",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Departments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Region",
                value: "Central");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                column: "Region",
                value: "Southern");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Region", "Zone" },
                values: new object[] { "Northern", "Mzimba" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                column: "Region",
                value: "Southern");

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "HeadOfficerBadge", "IsActive", "Name", "Region", "Zone" },
                values: new object[,]
                {
                    { 5, null, true, "Chitipa Traffic", "Northern", "Chitipa" },
                    { 6, null, true, "Karonga Traffic", "Northern", "Karonga" },
                    { 7, null, true, "Likoma Traffic", "Northern", "Likoma" },
                    { 8, null, true, "Nkhata Bay Traffic", "Northern", "Nkhata Bay" },
                    { 9, null, true, "Rumphi Traffic", "Northern", "Rumphi" },
                    { 10, null, true, "Dedza Traffic", "Central", "Dedza" },
                    { 11, null, true, "Dowa Traffic", "Central", "Dowa" },
                    { 12, null, true, "Kasungu Traffic", "Central", "Kasungu" },
                    { 13, null, true, "Mchinji Traffic", "Central", "Mchinji" },
                    { 14, null, true, "Nkhotakota Traffic", "Central", "Nkhotakota" },
                    { 15, null, true, "Ntcheu Traffic", "Central", "Ntcheu" },
                    { 16, null, true, "Ntchisi Traffic", "Central", "Ntchisi" },
                    { 17, null, true, "Salima Traffic", "Central", "Salima" },
                    { 18, null, true, "Balaka Traffic", "Southern", "Balaka" },
                    { 19, null, true, "Chikwawa Traffic", "Southern", "Chikwawa" },
                    { 20, null, true, "Chiradzulu Traffic", "Southern", "Chiradzulu" },
                    { 21, null, true, "Machinga Traffic", "Southern", "Machinga" },
                    { 22, null, true, "Mangochi Traffic", "Southern", "Mangochi" },
                    { 23, null, true, "Mulanje Traffic", "Southern", "Mulanje" },
                    { 24, null, true, "Mwanza Traffic", "Southern", "Mwanza" },
                    { 25, null, true, "Neno Traffic", "Southern", "Neno" },
                    { 26, null, true, "Nsanje Traffic", "Southern", "Nsanje" },
                    { 27, null, true, "Phalombe Traffic", "Southern", "Phalombe" },
                    { 28, null, true, "Thyolo Traffic", "Southern", "Thyolo" }
                });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 4,
                column: "Zone",
                value: "Mzimba");

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Code", "ConsecutiveFailedSyncs", "ContactPhone", "CreatedAt", "DepartmentId", "IsActive", "LastSyncAt", "Lat", "Lng", "Name", "OfficerInChargeBadge", "PhysicalAddress", "StationServerEndpoint", "StationServerPublicKey", "UpdatedAt", "Zone" },
                values: new object[,]
                {
                    { 6, "STN-CHI-001", 0, "+265 1 382 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, true, null, null, null, "Chitipa Police Station", null, "Chitipa Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chitipa" },
                    { 7, "STN-KAR-001", 0, "+265 1 362 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, true, null, null, null, "Karonga Police Station", null, "Karonga Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Karonga" },
                    { 8, "STN-LIK-001", 0, "+265 1 374 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, true, null, null, null, "Likoma Island Police Post", null, "Chizumulu Harbour", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Likoma" },
                    { 9, "STN-NKB-001", 0, "+265 1 352 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, true, null, null, null, "Nkhata Bay Police Station", null, "Nkhata Bay Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nkhata Bay" },
                    { 10, "STN-RUM-001", 0, "+265 1 372 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, true, null, null, null, "Rumphi Police Station", null, "Rumphi Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rumphi" },
                    { 11, "STN-DED-001", 0, "+265 1 223 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, true, null, null, null, "Dedza Police Station", null, "Dedza Boma, M1", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dedza" },
                    { 12, "STN-DOW-001", 0, "+265 1 282 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, true, null, null, null, "Dowa Police Station", null, "Dowa Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dowa" },
                    { 13, "STN-KAS-001", 0, "+265 1 253 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 12, true, null, null, null, "Kasungu Police Station", null, "Kasungu Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kasungu" },
                    { 14, "STN-MCH-001", 0, "+265 1 242 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, true, null, null, null, "Mchinji Border Police Station", null, "Mchinji Border (Zambia)", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mchinji" },
                    { 15, "STN-NKK-001", 0, "+265 1 292 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 14, true, null, null, null, "Nkhotakota Police Station", null, "Nkhotakota Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nkhotakota" },
                    { 16, "STN-NTC-001", 0, "+265 1 235 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15, true, null, null, null, "Ntcheu Police Station", null, "Ntcheu Boma, M1", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ntcheu" },
                    { 17, "STN-NTI-001", 0, "+265 1 295 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 16, true, null, null, null, "Ntchisi Police Station", null, "Ntchisi Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ntchisi" },
                    { 18, "STN-SAL-001", 0, "+265 1 263 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, true, null, null, null, "Salima Police Station", null, "Salima Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Salima" },
                    { 19, "STN-BAL-001", 0, "+265 1 552 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 18, true, null, null, null, "Balaka Police Station", null, "Balaka Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Balaka" },
                    { 20, "STN-CHK-001", 0, "+265 1 422 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 19, true, null, null, null, "Chikwawa Police Station", null, "Chikwawa Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chikwawa" },
                    { 21, "STN-CHZ-001", 0, "+265 1 462 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 20, true, null, null, null, "Chiradzulu Police Station", null, "Chiradzulu Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chiradzulu" },
                    { 22, "STN-MAC-001", 0, "+265 1 535 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 21, true, null, null, null, "Machinga Police Station", null, "Liwonde Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Machinga" },
                    { 23, "STN-MAN-001", 0, "+265 1 584 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 22, true, null, null, null, "Mangochi Police Station", null, "Mangochi Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mangochi" },
                    { 24, "STN-MUL-001", 0, "+265 1 466 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 23, true, null, null, null, "Mulanje Police Station", null, "Mulanje Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mulanje" },
                    { 25, "STN-MWA-001", 0, "+265 1 432 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 24, true, null, null, null, "Mwanza Border Police Station", null, "Mwanza Border (Mozambique)", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mwanza" },
                    { 26, "STN-NEN-001", 0, "+265 1 433 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 25, true, null, null, null, "Neno Police Station", null, "Neno Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Neno" },
                    { 27, "STN-NSA-001", 0, "+265 1 451 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 26, true, null, null, null, "Nsanje Police Station", null, "Nsanje Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nsanje" },
                    { 28, "STN-PHA-001", 0, "+265 1 467 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 27, true, null, null, null, "Phalombe Police Station", null, "Phalombe Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Phalombe" },
                    { 29, "STN-THY-001", 0, "+265 1 473 100", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 28, true, null, null, null, "Thyolo Police Station", null, "Thyolo Boma", null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thyolo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Region",
                table: "Departments",
                column: "Region");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departments_Region",
                table: "Departments");

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "HeadOfficerBadge",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                column: "Zone",
                value: "Mzuzu");

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 4,
                column: "Zone",
                value: "Mzuzu");
        }
    }
}
