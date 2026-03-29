using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "MemberIdentifier",
                table: "Approvals",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BatchDueDate", "BatchDueDays", "ReceiveDate" },
                values: new object[] { new DateTime(2024, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArabicName", "EnglishName", "RefundDueDays", "ShortName" },
                values: new object[] { "شركة الأعمال المتقدمة", "Advanced Business Corp", 30, "ABC" });

            migrationBuilder.InsertData(
                table: "MemberPolicyInfos",
                columns: new[] { "Id", "AddDate", "Address", "AppPassword", "BranchId", "CardPrinted", "CodeAtCompany", "CreatedAt", "CreatedBy", "DeleteDate", "Email", "FirebaseToken", "HeadOfFamilyId", "ImageUrl", "IsExpired", "IsHr", "IsVip", "JobTitle", "MemberId", "Notes", "PolicyId", "ProgramId", "RelationId", "TotalApprovals", "TotalClaims", "TotalRefund", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, false, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", null, null, null, null, null, false, false, false, null, 1, null, 1, 1, 1, 0, 0, 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder" });

            migrationBuilder.InsertData(
                table: "Pools",
                columns: new[] { "Id", "Name", "PolicyId" },
                values: new object[,]
                {
                    { 1, "Medication Pool", 1 },
                    { 2, "Dental Pool", 1 }
                });

            migrationBuilder.InsertData(
                table: "Programs",
                columns: new[] { "Id", "NameAr", "NameEn", "PolicyId" },
                values: new object[,]
                {
                    { 1, "برنامج العيادات الخارجية", "Outpatient Program", 1 },
                    { 2, "برنامج الأسنان", "Dental Program", 1 }
                });

            migrationBuilder.InsertData(
                table: "ProviderAttachments",
                columns: new[] { "Id", "FileName", "FilePath", "FileType", "ProviderId" },
                values: new object[] { 1, "license.pdf", "/uploads/providers/1/license.pdf", "application/pdf", 1 });

            migrationBuilder.InsertData(
                table: "ProviderDiscounts",
                columns: new[] { "Id", "DiscountType", "ProviderId", "Value" },
                values: new object[] { 1, "Percentage", 1, 10m });

            migrationBuilder.InsertData(
                table: "ProviderFinancialData",
                columns: new[] { "Id", "AccountNumber", "BankName", "Iban", "ProviderId", "SwiftCode" },
                values: new object[] { 1, "123456789012", "National Bank of Egypt", "EG380019000100116001200180", 1, "NBEGEGCX" });

            migrationBuilder.InsertData(
                table: "ProviderLocations",
                columns: new[] { "Id", "AllowChronic", "CityId", "GoogleMapsUrl", "GovernmentId", "PortalEmail", "PortalPassword", "PrimaryLandline", "PrimaryMobile", "ProviderId", "SecondaryLandline", "SecondaryMobile", "StreetAr", "StreetEn" },
                values: new object[] { 1, true, 1, "https://maps.google.com/?q=30.0444,31.2357", 1, null, null, "02-12345678", "01200000001", 1, null, null, "شارع الجمهورية", "Gomhoria Street" });

            migrationBuilder.InsertData(
                table: "ProviderPriceLists",
                columns: new[] { "Id", "Price", "ProviderId", "ServiceName" },
                values: new object[,]
                {
                    { 1, 200m, 1, "General Consultation" },
                    { 2, 150m, 1, "Lab Test" }
                });

            migrationBuilder.InsertData(
                table: "RefundRules",
                columns: new[] { "Id", "PolicyId", "Rule" },
                values: new object[,]
                {
                    { 1, 1, "80% refund for pharmacies up to 1000 EGP" },
                    { 2, 1, "70% refund for clinics up to 800 EGP" }
                });

            migrationBuilder.InsertData(
                table: "Approvals",
                columns: new[] { "Id", "AdditionalPool", "ChronicForDate", "ClaimFormNumber", "Comment", "CreatedAt", "CreatedBy", "Diagnosis", "EmailOrPhone", "InternalNote", "IsApproved", "IsDebit", "IsDelivery", "IsFromProviderPortal", "IsRepeated", "MaxAllowedAmount", "MemberIdentifier", "MemberPolicyInfoId", "ProviderBranch", "ProviderId", "ProviderName", "ReceiveDate", "ReceiveTime", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, null, null, "CF-2025-001", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Chronic diabetes medication", null, null, true, false, false, true, true, 500.00m, "123456789", 1, "Main Branch", 1, "City Pharmacy", new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 10, 30, 0, 0), null, null });

            migrationBuilder.InsertData(
                table: "Approvals",
                columns: new[] { "Id", "AdditionalPool", "ChronicForDate", "ClaimFormNumber", "Comment", "CreatedAt", "CreatedBy", "Diagnosis", "EmailOrPhone", "InternalNote", "IsDebit", "IsDelivery", "IsRepeated", "MaxAllowedAmount", "MemberIdentifier", "MemberPolicyInfoId", "ProviderBranch", "ProviderId", "ProviderName", "ReceiveDate", "ReceiveTime", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 2, null, null, "CF-2025-002", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Hypertension treatment", null, null, false, true, false, 350.00m, "987654321", 1, "Downtown", 1, "Health Pharmacy", new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 14, 15, 0, 0), null, null });

            migrationBuilder.InsertData(
                table: "Approvals",
                columns: new[] { "Id", "AdditionalPool", "ChronicForDate", "ClaimFormNumber", "Comment", "CreatedAt", "CreatedBy", "Diagnosis", "EmailOrPhone", "InternalNote", "IsApproved", "IsDebit", "IsDelivery", "IsDispensed", "IsFromProviderPortal", "IsRepeated", "MaxAllowedAmount", "MemberIdentifier", "MemberPolicyInfoId", "ProviderBranch", "ProviderId", "ProviderName", "ReceiveDate", "ReceiveTime", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 3, null, null, "CF-2025-003", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Asthma medication", null, null, true, true, false, true, true, true, 450.00m, "555666777", 1, "North Branch", 1, "Care Pharmacy", new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 9, 0, 0, 0), null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Approvals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Approvals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Approvals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Pools",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pools",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Programs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Programs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProviderAttachments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProviderDiscounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProviderFinancialData",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProviderLocations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProviderPriceLists",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProviderPriceLists",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RefundRules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RefundRules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MemberPolicyInfos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "MemberIdentifier",
                table: "Approvals",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);

            migrationBuilder.UpdateData(
                table: "Batches",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BatchDueDate", "BatchDueDays", "ReceiveDate" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArabicName", "EnglishName", "RefundDueDays", "ShortName" },
                values: new object[] { "شركة التقنية المتقدمة", "Advanced Technology Co.", 5, "ATC" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "ActivePolicyId", "ArabicName", "CategoryId", "EnglishName", "ImageUrl", "PolicyExpire", "PolicyStart", "RefundDueDays", "ShortName", "Status", "Type" },
                values: new object[] { 2, null, "مستشفى الشفاء", 3, "Al Shifa Hospital", null, null, null, 7, "ASH", "Active", "Corporate" });
        }
    }
}
