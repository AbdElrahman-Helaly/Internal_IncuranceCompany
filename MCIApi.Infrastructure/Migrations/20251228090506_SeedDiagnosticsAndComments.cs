using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDiagnosticsAndComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalMedicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ReasonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalMedicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalMedicines_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalMedicines_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalMedicines_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMedicines_ApprovalId",
                table: "ApprovalMedicines",
                column: "ApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMedicines_MedicineId",
                table: "ApprovalMedicines",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalMedicines_UnitId",
                table: "ApprovalMedicines",
                column: "UnitId");

            // Insert Diagnostics seed data (only if they don't exist)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Diagnostics] WHERE [Id] = 1)
                BEGIN
                    SET IDENTITY_INSERT [Diagnostics] ON;
                    INSERT INTO [Diagnostics] ([Id], [Code], [NameAr], [NameEn], [Description], [IsDeleted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy])
                    VALUES 
                        (1, N'E11.9', N'داء السكري من النوع 2 بدون مضاعفات', N'Type 2 diabetes mellitus without complications', N'Type 2 diabetes without complications', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (2, N'I10', N'ارتفاع ضغط الدم الأساسي', N'Essential (primary) hypertension', N'Primary hypertension without known cause', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (3, N'J06.9', N'عدوى الجهاز التنفسي العلوي الحادة غير محددة', N'Acute upper respiratory infection, unspecified', N'Acute upper respiratory infection', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (4, N'K21.9', N'مرض الارتجاع المعدي المريئي بدون التهاب المريء', N'Gastro-esophageal reflux disease without esophagitis', N'GERD without esophagitis', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (5, N'M79.3', N'ألم في الأنسجة الرخوة، غير محدد', N'Panniculitis, unspecified', N'Soft tissue pain, unspecified', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (6, N'E78.5', N'فرط شحوم الدم غير محدد', N'Hyperlipidemia, unspecified', N'High cholesterol/lipids', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (7, N'Z51.1', N'جلسات العلاج الكيميائي للورم', N'Chemotherapy session for neoplasm', N'Chemotherapy treatment', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (8, N'F41.9', N'اضطراب القلق غير محدد', N'Anxiety disorder, unspecified', N'Anxiety disorder', 0, '2024-01-01', N'Seeder', NULL, NULL);
                    SET IDENTITY_INSERT [Diagnostics] OFF;
                END
            ");

            // Insert Comments seed data (only if they don't exist)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Comments] WHERE [Id] = 1)
                BEGIN
                    SET IDENTITY_INSERT [Comments] ON;
                    INSERT INTO [Comments] ([Id], [TextAr], [TextEn], [Description], [IsDeleted], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy])
                    VALUES 
                        (1, N'يحتاج إلى موافقة', N'Approval needed', N'This approval requires additional review and approval', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (2, N'حالة طارئة', N'Urgent case', N'This is an urgent case that requires immediate attention', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (3, N'مطلوب مستندات إضافية', N'Additional documents required', N'Please provide additional documents to complete the approval process', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (4, N'موافقة مسبقة', N'Pre-approval', N'Pre-approval has been granted for this case', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (5, N'قيد المراجعة', N'Under review', N'This case is currently under review by the medical team', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (6, N'تمت الموافقة', N'Approved', N'This approval has been approved and processed', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (7, N'تم الرفض', N'Rejected', N'This approval has been rejected due to policy limitations', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (8, N'في انتظار المعلومات', N'Pending information', N'Waiting for additional information from the provider', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (9, N'يغطيها التأمين', N'Covered by insurance', N'This service/medicine is covered by the insurance policy', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (10, N'لا يغطيها التأمين', N'Not covered by insurance', N'This service/medicine is not covered by the insurance policy', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (11, N'يحتاج إلى ترخيص', N'Requires authorization', N'This case requires special authorization from the medical committee', 0, '2024-01-01', N'Seeder', NULL, NULL),
                        (12, N'موافقة قياسية', N'Standard approval', N'Standard approval process completed successfully', 0, '2024-01-01', N'Seeder', NULL, NULL);
                    SET IDENTITY_INSERT [Comments] OFF;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete Comments seed data (only if they exist)
            migrationBuilder.Sql(@"
                DELETE FROM [Comments] WHERE [Id] BETWEEN 1 AND 12;
            ");

            // Delete Diagnostics seed data (only if they exist)
            migrationBuilder.Sql(@"
                DELETE FROM [Diagnostics] WHERE [Id] BETWEEN 1 AND 8;
            ");

            migrationBuilder.DropTable(
                name: "ApprovalMedicines");
        }
    }
}
