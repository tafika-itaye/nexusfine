using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusFine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Module35_OffenceCodes_GN38_2026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Exceeding the posted speed limit. K20,000–K90,000 depending on excess speed.");

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 50000m, "Disregarding traffic signs, signals or markings (including red-light violations).", "Failure to Obey Road Signs" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DefaultFineAmount", "Description" },
                values: new object[] { 15000m, "Driver or passenger not wearing seatbelt. K15,000 per affected passenger." });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 100000m, "Operating a vehicle not registered with DRTSS, or with an expired registration.", "Using an Unregistered Vehicle" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DefaultFineAmount", "Description" },
                values: new object[] { 50000m, "Operating a vehicle in an unroadworthy condition." });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 50000m, "Overtaking in a manner likely to cause danger to other road users.", "Dangerous Overtaking" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 30000m, "Motorcycle rider or passenger without an approved helmet.", "Riding Motorcycle Without Helmet" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 100000m, "Carrying goods/load that are not secured. Separate K50,000 fine for throwing rubbish from a vehicle.", "Carrying Unsecured Goods or Load" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 200000m, "Operating a motor vehicle without a valid driver's licence.", "Driving Without a Licence" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Holding or operating a mobile phone while driving.", "Using Phone While Driving" });

            migrationBuilder.InsertData(
                table: "OffenceCodes",
                columns: new[] { "Id", "Code", "DefaultFineAmount", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 11, "OC-011", 100000m, "Number plates or vehicle identification not properly displayed.", true, "Failure to Display Number Plates" },
                    { 12, "OC-012", 300000m, "Driving while under the influence of alcohol. Up to K300,000 plus driver's-licence suspension.", true, "Drunk Driving" },
                    { 13, "OC-013", 100000m, "Driving without valid third-party insurance — K100,000 (private), up to K800,000 for PSV.", true, "No Insurance (Private Vehicle)" },
                    { 14, "OC-014", 50000m, "Vehicle operating without a valid Certificate of Fitness.", true, "No Certificate of Fitness (COF)" },
                    { 15, "OC-015", 30000m, "Parking in a prohibited area or in a manner that obstructs other road users.", true, "Parking Illegally or Causing Obstruction" },
                    { 16, "OC-016", 50000m, "Failure to stop when signalled to do so by a uniformed traffic officer.", true, "Failing to Stop for Traffic Officers" },
                    { 17, "OC-017", 20000m, "Carrying more passengers than permitted on a motorcycle. K20,000 per excess passenger.", true, "Excess Passengers on Motorcycle" },
                    { 18, "OC-018", 30000m, "Sounding the vehicle horn other than as a warning of imminent danger.", true, "Hooting Unnecessarily" },
                    { 19, "OC-019", 50000m, "Reading, sending or composing text messages, or making/receiving calls while driving.", true, "Texting or Calling While Driving" },
                    { 20, "OC-020", 10000m, "Vehicle missing one or more required safety items. K10,000 per missing item.", true, "No Warning Triangles / Fire Extinguisher / Spare Tyre" },
                    { 21, "OC-021", 50000m, "Discarding waste from a moving or stationary vehicle onto a public road.", true, "Throwing Rubbish from Vehicle" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Exceeding the posted speed limit");

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 75000m, "Failure to stop at a red traffic light", "Red Light Violation" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DefaultFineAmount", "Description" },
                values: new object[] { 25000m, "Driver or passenger not wearing seatbelt" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 40000m, "Driving with an expired road licence", "Expired Road Licence" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DefaultFineAmount", "Description" },
                values: new object[] { 60000m, "Operating a vehicle in an unroadworthy condition" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 30000m, "Driving in the wrong lane", "Wrong Lane" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 20000m, "Motorcycle rider without a helmet", "No Helmet" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 80000m, "Vehicle carrying load exceeding permitted weight", "Overloading" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DefaultFineAmount", "Description", "Name" },
                values: new object[] { 100000m, "Operating a vehicle without a valid licence", "Driving Without Licence" });

            migrationBuilder.UpdateData(
                table: "OffenceCodes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Using a handheld mobile phone while driving", "Use of Mobile Phone While Driving" });
        }
    }
}
