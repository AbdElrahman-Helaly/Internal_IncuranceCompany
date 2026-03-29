using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HandleDuplicateMobileNumberValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProviderLocations",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProviderLocations",
                columns: new[] { "Id", "AllowChronic", "CityId", "GoogleMapsUrl", "GovernmentId", "PortalEmail", "PortalPassword", "PrimaryLandline", "PrimaryMobile", "ProviderId", "SecondaryLandline", "SecondaryMobile", "StreetAr", "StreetEn" },
                values: new object[] { 1, true, 1, "https://maps.google.com/?q=30.0444,31.2357", 1, null, null, "02-12345678", "01200000001", 1, null, null, "شارع الجمهورية", "Gomhoria Street" });
        }
    }
}
