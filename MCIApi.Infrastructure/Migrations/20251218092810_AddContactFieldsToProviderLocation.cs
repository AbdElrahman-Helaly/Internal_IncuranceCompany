using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactFieldsToProviderLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ProviderLocations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hotline",
                table: "ProviderLocations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "ProviderLocations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "ProviderLocations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ProviderLocations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Hotline", "Mobile", "Telephone" },
                values: new object[] { null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "Hotline",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "ProviderLocations");
        }
    }
}
