using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalUnitsFromImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دهان مهبلي", "VAGINAL PAINT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص مهبلي", "VAGINAL TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دوش مهبلي", "VAGINAL DOUCHE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة صلبة", "CAPLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رول أون", "ROLL ON" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دهان موضعي", "TOPICAL PAINT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مرهم عيني", "EYE OINTMENT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل", "GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "زيت", "OIL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مستلزمات طبية", "MEDICAL SUPPLIES" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "تحميلة مهبلية", "VAGINAL SUPPOSITORY" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كريم مهبلي", "VAGINAL CREAM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رذاذ موضعي", "TOPICAL SPRAY" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "محلول موضعي", "TOPICAL SOLUTION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "شريط غذائي", "NUTRITION BAR" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "تسريب", "INFUSION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل شرجي", "RECTAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل فموي", "ORAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مرهم", "OINTMENT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل مهبلي", "VAGINAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة جيلاتينية صلبة", "HARD GELATINE CAPSULE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "حبة", "PILL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دعامة", "BRACE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "علكة", "GUM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مسحوق فموي", "ORAL POWDER" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رذاذ فموي", "MOUTH SPRAY" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "صابون", "SOAP" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل عيني", "EYE GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "معجون صالح للأكل", "EDIBLE PASTE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "محلول استنشاق", "INHALATION SOLUTION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كريم", "CREAM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "معلق", "SUSPENSION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "أقراص", "PASTILLE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قطرات أنفية", "NASAL DROPS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قطرات عين وأذن", "EYE EAR DROPS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "ماء", "WATER" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مسحوق موضعي", "TOPICAL POWDER" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل أنفي", "NASAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رغوة", "FOAM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "زرع", "IMPLANT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "سائل فموي", "ORAL LIQUID" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص قابل للتفريق", "DISPERSABLE TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "غسول مهبلي", "VAGINAL WASH" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "حزام", "BELT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "محلول فموي", "ORAL SOLUTION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة جيلاتينية ناعمة", "SOFT GELATINE CAPSULE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مضغات ناعمة", "SOFT CHEWS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة سائلة", "LIQUICAP" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص قابل للذوبان", "SOLUBLE TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص قابل للمضغ", "CHEWABLE TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قطرات فموية", "ORAL DROPS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة استنشاق", "INHALATION CAPSULE" });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 96, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", "دهان فموي", "ORAL PAINT", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص مهبلي", "VAGINAL TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دش مهبلي", "VAGINAL DOUCHE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة صلبة", "CAPLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رول أون", "ROLL ON" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دهان موضعي", "TOPICAL PAINT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مرهم عيني", "EYE OINTMENT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل", "GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "زيت", "OIL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مستلزمات طبية", "MEDICAL SUPPLIES" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "تحميلة مهبلية", "VAGINAL SUPPOSITORY" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كريم مهبلي", "VAGINAL CREAM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رذاذ موضعي", "TOPICAL SPRAY" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "محلول موضعي", "TOPICAL SOLUTION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "شريط غذائي", "NUTRITION BAR" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "تسريب", "INFUSION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل شرجي", "RECTAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل فموي", "ORAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مرهم", "OINTMENT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل مهبلي", "VAGINAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة جيلاتينية صلبة", "HARD GELATINE CAPSULE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "حبة", "PILL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دعامة", "BRACE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "علكة", "GUM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مسحوق فموي", "ORAL POWDER" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رذاذ فموي", "MOUTH SPRAY" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "صابون", "SOAP" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل عيني", "EYE GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "معجون صالح للأكل", "EDIBLE PASTE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "محلول استنشاق", "INHALATION SOLUTION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كريم", "CREAM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "معلق", "SUSPENSION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "أقراص استحلاب", "PASTILLE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قطرات أنفية", "NASAL DROPS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قطرات عين وأذن", "EYE EAR DROPS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "ماء", "WATER" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مسحوق موضعي", "TOPICAL POWDER" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "جل أنفي", "NASAL GEL" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "رغوة", "FOAM" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "زرع", "IMPLANT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "سائل فموي", "ORAL LIQUID" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص قابل للتفريق", "DISPERSABLE TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "غسول مهبلي", "VAGINAL WASH" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "حزام", "BELT" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "محلول فموي", "ORAL SOLUTION" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة جيلاتينية ناعمة", "SOFT GELATINE CAPSULE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "مضغ ناعم", "SOFT CHEWS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة سائلة", "LIQUICAP" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص قابل للذوبان", "SOLUBLE TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قرص قابل للمضغ", "CHEWABLE TABLET" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "قطرات فموية", "ORAL DROPS" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "كبسولة استنشاق", "INHALATION CAPSULE" });

            migrationBuilder.UpdateData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "NameAr", "NameEn" },
                values: new object[] { "دهان فموي", "ORAL PAINT" });
        }
    }
}
