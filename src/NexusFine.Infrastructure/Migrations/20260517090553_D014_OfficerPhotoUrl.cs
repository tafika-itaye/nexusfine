using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusFine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class D014_OfficerPhotoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Officers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Officers");
        }
    }
}
