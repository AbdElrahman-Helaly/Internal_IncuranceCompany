using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Ini56t : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Corporate" },
                    { 2, "Individual" },
                    { 3, "Healthcare" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "تكنولوجيا المعلومات", "IT" },
                    { 2, "المبيعات", "Sales" }
                });

            migrationBuilder.InsertData(
                table: "InsuranceCompanies",
                columns: new[] { "Id", "ArName", "CreatedAt", "CreatedBy", "EnName", "ImagePath", "IsDeleted", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "شركة التأمين الوطنية", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "National Insurance Co.", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, "شركة التأمين المتحدة", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "United Insurance", null, false, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                table: "JobTitles",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "مدير النظام", "System Administrator" },
                    { 2, "محاسب", "Accountant" }
                });

            migrationBuilder.InsertData(
                table: "ProviderCategories",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "مستشفيات", "Hospitals" },
                    { 2, "عيادات", "Clinics" },
                    { 3, "صيدليات", "Pharmacies" }
                });

            migrationBuilder.InsertData(
                table: "Relations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Father" },
                    { 2, "Mother" },
                    { 3, "Child" },
                    { 4, "Spouse" }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "ActivePolicyId", "ArabicName", "CategoryId", "EnglishName", "ImageUrl", "PolicyExpire", "PolicyStart", "RefundDueDays", "ShortName", "Status", "Type" },
                values: new object[,]
                {
                    { 1, null, "شركة التقنية المتقدمة", 1, "Advanced Technology Co.", null, null, null, 5, "ATC", "Active", "Corporate" },
                    { 2, null, "مستشفى الشفاء", 3, "Al Shifa Hospital", null, null, null, 7, "ASH", "Active", "Corporate" }
                });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "Id", "BatchDueDays", "CategoryId", "CommercialName", "CommercialRegisterNumber", "Email", "Fax", "HasAPortal", "Hotline", "ImagePath", "ImageUrl", "IsActive", "IsDeleted", "NameAr", "NameEn", "NetworkClass", "Online", "Phone", "Priority", "Status", "VATNumber" },
                values: new object[] { 1, (short)30, 1, null, "CR-987654", "provider@example.com", "02-1234567", true, "19000", null, null, true, false, "مقدم الخدمة النموذجي", "Sample Provider", "A", "Yes", "01200000000", "A", "Active", "123456789" });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "ArabicName", "ClientId", "EnglishName", "Status" },
                values: new object[,]
                {
                    { 1, "الفرع الرئيسي", 1, "Main Branch", "Active" },
                    { 2, "فرع الإسكندرية", 1, "Alex Branch", "Active" }
                });

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedBy", "EndDate", "InsuranceCompanyId", "IsDeleted", "IsManagement", "StartDate", "Status", "TotalAmount", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 50000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder" });

            migrationBuilder.InsertData(
                table: "GeneralPrograms",
                columns: new[] { "Id", "ArName", "EnName", "PolicyId" },
                values: new object[,]
                {
                    { 1, "برنامج الصحة", "Health", 1 },
                    { 2, "برنامج السفر", "Travel", 1 }
                });

            migrationBuilder.InsertData(
                table: "MemberInfos",
                columns: new[] { "Id", "ActivatedDate", "BirthDate", "BranchId", "CreatedAt", "CreatedBy", "FirstName", "FullName", "IsMale", "JobTitle", "LastName", "MemberImage", "MiddleName", "MobileNumber", "NationalId", "Notes", "PrivateNotes", "Status", "UpdatedAt", "UpdatedBy", "VipStatus" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "Ahmed", "Ahmed Ali Hassan", true, "Software Engineer", "Hassan", "/images/members/ahmed.png", "Ali", "01111111111", "29001010123456", "Primary member", "VIP", "Active", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "No" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "GeneralPrograms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GeneralPrograms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InsuranceCompanies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "JobTitles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "JobTitles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MemberInfos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Relations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProviderCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InsuranceCompanies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
