using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedProviderCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مركز أسنان", "Dental Center" });

            migrationBuilder.UpdateData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "طبيب", "Doctor" });

            migrationBuilder.UpdateData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مستشفى", "Hospital" });

            migrationBuilder.InsertData(
                table: "ProviderCategories",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 4, "معمل", "Lab" },
                    { 5, "مركز بصريات", "Optical Center" },
                    { 6, "صيدلية", "Pharmacy" },
                    { 7, "مركز علاج طبيعي", "PhysioTherapy Center" },
                    { 8, "مركز أشعة", "Scan Center" },
                    { 9, "مركز متخصص", "Specialized Center" },
                    { 10, "عيادات متخصصة", "Specialized clinics" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مستشفيات", "Hospitals" });

            migrationBuilder.UpdateData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "عيادات", "Clinics" });

            migrationBuilder.UpdateData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "صيدليات", "Pharmacies" });
        }
    }
}
