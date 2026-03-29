using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceProviderLocationAreaNameIdsWithNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaNameArId",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "AreaNameEnId",
                table: "ProviderLocations");

            migrationBuilder.AddColumn<string>(
                name: "AreaNameAr",
                table: "ProviderLocations",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaNameEn",
                table: "ProviderLocations",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ProviderLocations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AreaNameAr", "AreaNameEn" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaNameAr",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "AreaNameEn",
                table: "ProviderLocations");

            migrationBuilder.AddColumn<int>(
                name: "AreaNameArId",
                table: "ProviderLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AreaNameEnId",
                table: "ProviderLocations",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ProviderLocations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AreaNameArId", "AreaNameEnId" },
                values: new object[] { null, null });
        }
    }
}
