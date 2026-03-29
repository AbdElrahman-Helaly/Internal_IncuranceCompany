using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreUnitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 43, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "شامبو", "SHAMPOO", null, null },
                    { 44, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "حزام", "BELT", null, null },
                    { 45, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "محلول فموي", "ORAL SOLUTION", null, null },
                    { 46, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كبسولة جيلاتينية ناعمة", "SOFT GELATINE CAPSULE", null, null },
                    { 47, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "مضغات ناعمة", "SOFT CHEWS", null, null },
                    { 48, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كبسولة سائلة", "LIQUICAP", null, null },
                    { 49, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قرص قابل للذوبان", "SOLUBLE TABLET", null, null },
                    { 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قرص قابل للمضغ", "CHEWABLE TABLET", null, null },
                    { 51, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قطرات فموية", "ORAL DROPS", null, null },
                    { 52, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كبسولة استنشاق", "INHALATION CAPSULE", null, null },
                    { 53, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "دهان فموي", "ORAL PAINT", null, null },
                    { 54, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "دهان مهبلي", "VAGINAL PAINT", null, null },
                    { 55, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قرص مهبلي", "VAGINAL TABLET", null, null },
                    { 56, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غسول مهبلي", "VAGINAL DOUCHE", null, null },
                    { 57, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كبسولة", "CAPLET", null, null },
                    { 58, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "رول أون", "ROLL ON", null, null },
                    { 59, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "دهان موضعي", "TOPICAL PAINT", null, null },
                    { 60, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "مرهم عيني", "EYE OINTMENT", null, null },
                    { 61, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جل", "GEL", null, null },
                    { 62, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "زيت", "OIL", null, null },
                    { 63, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "مستلزمات طبية", "MEDICAL SUPPLIES", null, null },
                    { 64, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "تحميلة مهبلية", "VAGINAL SUPPOSITORY", null, null },
                    { 65, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كريم مهبلي", "VAGINAL CREAM", null, null },
                    { 66, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "رذاذ موضعي", "TOPICAL SPRAY", null, null },
                    { 67, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "محلول موضعي", "TOPICAL SOLUTION", null, null },
                    { 68, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "شريط غذائي", "NUTRITION BAR", null, null },
                    { 69, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "تسريب", "INFUSION", null, null },
                    { 70, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جل شرجي", "RECTAL GEL", null, null },
                    { 71, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جل فموي", "ORAL GEL", null, null },
                    { 72, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "مرهم", "OINTMENT", null, null },
                    { 73, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جل مهبلي", "VAGINAL GEL", null, null },
                    { 74, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كبسولة جيلاتينية صلبة", "HARD GELATINE CAPSULE", null, null },
                    { 75, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "حبة", "PILL", null, null },
                    { 76, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "دعامة", "BRACE", null, null },
                    { 77, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "علكة", "GUM", null, null },
                    { 78, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "مسحوق فموي", "ORAL POWDER", null, null },
                    { 79, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "رذاذ فموي", "MOUTH SPRAY", null, null },
                    { 80, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "صابون", "SOAP", null, null },
                    { 81, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جل عيني", "EYE GEL", null, null },
                    { 82, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "معجون صالح للأكل", "EDIBLE PASTE", null, null },
                    { 83, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "محلول استنشاق", "INHALATION SOLUTION", null, null },
                    { 84, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "كريم", "CREAM", null, null },
                    { 85, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "معلق", "SUSPENSION", null, null },
                    { 86, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "أقراص", "PASTILLE", null, null },
                    { 87, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قطرات أنفية", "NASAL DROPS", null, null },
                    { 88, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قطرات عين وأذن", "EYE EAR DROPS", null, null },
                    { 89, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "ماء", "WATER", null, null },
                    { 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "مسحوق موضعي", "TOPICAL POWDER", null, null },
                    { 91, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "جل أنفي", "NASAL GEL", null, null },
                    { 92, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "رغوة", "FOAM", null, null },
                    { 93, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "زرع", "IMPLANT", null, null },
                    { 94, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "سائل فموي", "ORAL LIQUID", null, null },
                    { 95, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "قرص قابل للتشتت", "DISPERSABLE TABLET", null, null },
                    { 96, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "غسول مهبلي", "VAGINAL WASH", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 96);
        }
    }
}
