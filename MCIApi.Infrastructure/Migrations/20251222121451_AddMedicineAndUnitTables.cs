using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicineAndUnitTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_MemberPolicyInfos_MemberPolicyInfoId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_MemberIdentifier",
                table: "Approvals");

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

            migrationBuilder.DropColumn(
                name: "AdditionalPool",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "EmailOrPhone",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "MemberIdentifier",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ProviderName",
                table: "Approvals");

            migrationBuilder.RenameColumn(
                name: "ProviderBranch",
                table: "Approvals",
                newName: "RequestEmailOrMobile");

            migrationBuilder.RenameColumn(
                name: "MemberPolicyInfoId",
                table: "Approvals",
                newName: "ProviderLocationId");

            migrationBuilder.RenameColumn(
                name: "MaxAllowedAmount",
                table: "Approvals",
                newName: "MaxAllowAmount");

            migrationBuilder.RenameIndex(
                name: "IX_Approvals_MemberPolicyInfoId",
                table: "Approvals",
                newName: "IX_Approvals_ProviderLocationId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRepeated",
                table: "Approvals",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDelivery",
                table: "Approvals",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDebit",
                table: "Approvals",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "AdditionalPoolId",
                table: "Approvals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Approvals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Approvals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdditionalPools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalPools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diagnostics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalAdditionalPools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    AdditionalPoolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalAdditionalPools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalAdditionalPools_AdditionalPools_AdditionalPoolId",
                        column: x => x.AdditionalPoolId,
                        principalTable: "AdditionalPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalAdditionalPools_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalDiagnostics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    DiagnosticId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalDiagnostics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalDiagnostics_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalDiagnostics_Diagnostics_DiagnosticId",
                        column: x => x.DiagnosticId,
                        principalTable: "Diagnostics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Unit1Id = table.Column<int>(type: "int", nullable: false),
                    Unit2Id = table.Column<int>(type: "int", nullable: false),
                    Unit1Count = table.Column<int>(type: "int", nullable: false),
                    Unit2Count = table.Column<int>(type: "int", nullable: false),
                    FullForm = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsLocal = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MedicinePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicines_Units_Unit1Id",
                        column: x => x.Unit1Id,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicines_Units_Unit2Id",
                        column: x => x.Unit2Id,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_AdditionalPoolId",
                table: "Approvals",
                column: "AdditionalPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_CommentId",
                table: "Approvals",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_MemberId",
                table: "Approvals",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAdditionalPools_AdditionalPoolId",
                table: "ApprovalAdditionalPools",
                column: "AdditionalPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAdditionalPools_ApprovalId_AdditionalPoolId",
                table: "ApprovalAdditionalPools",
                columns: new[] { "ApprovalId", "AdditionalPoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDiagnostics_ApprovalId_DiagnosticId",
                table: "ApprovalDiagnostics",
                columns: new[] { "ApprovalId", "DiagnosticId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDiagnostics_DiagnosticId",
                table: "ApprovalDiagnostics",
                column: "DiagnosticId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostics_Code",
                table: "Diagnostics",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_Unit1Id",
                table: "Medicines",
                column: "Unit1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_Unit2Id",
                table: "Medicines",
                column: "Unit2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_AdditionalPools_AdditionalPoolId",
                table: "Approvals",
                column: "AdditionalPoolId",
                principalTable: "AdditionalPools",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Comments_CommentId",
                table: "Approvals",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_MemberInfos_MemberId",
                table: "Approvals",
                column: "MemberId",
                principalTable: "MemberInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_ProviderLocations_ProviderLocationId",
                table: "Approvals",
                column: "ProviderLocationId",
                principalTable: "ProviderLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_AdditionalPools_AdditionalPoolId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Comments_CommentId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_MemberInfos_MemberId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_ProviderLocations_ProviderLocationId",
                table: "Approvals");

            migrationBuilder.DropTable(
                name: "ApprovalAdditionalPools");

            migrationBuilder.DropTable(
                name: "ApprovalDiagnostics");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "AdditionalPools");

            migrationBuilder.DropTable(
                name: "Diagnostics");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_AdditionalPoolId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_CommentId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_MemberId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "AdditionalPoolId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Approvals");

            migrationBuilder.RenameColumn(
                name: "RequestEmailOrMobile",
                table: "Approvals",
                newName: "ProviderBranch");

            migrationBuilder.RenameColumn(
                name: "ProviderLocationId",
                table: "Approvals",
                newName: "MemberPolicyInfoId");

            migrationBuilder.RenameColumn(
                name: "MaxAllowAmount",
                table: "Approvals",
                newName: "MaxAllowedAmount");

            migrationBuilder.RenameIndex(
                name: "IX_Approvals_ProviderLocationId",
                table: "Approvals",
                newName: "IX_Approvals_MemberPolicyInfoId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRepeated",
                table: "Approvals",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDelivery",
                table: "Approvals",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDebit",
                table: "Approvals",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalPool",
                table: "Approvals",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Approvals",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Approvals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailOrPhone",
                table: "Approvals",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberIdentifier",
                table: "Approvals",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProviderName",
                table: "Approvals",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.InsertData(
                table: "Approvals",
                columns: new[] { "Id", "AdditionalPool", "ChronicForDate", "ClaimFormNumber", "Comment", "CreatedAt", "CreatedBy", "Diagnosis", "EmailOrPhone", "InternalNote", "IsApproved", "IsDebit", "IsDelivery", "IsFromProviderPortal", "IsRepeated", "MaxAllowedAmount", "MemberIdentifier", "MemberPolicyInfoId", "ProviderBranch", "ProviderId", "ProviderName", "ReceiveDate", "ReceiveTime", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, null, null, "CF-2025-001", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "system", "Chronic diabetes medication", null, null, true, false, false, true, true, 500.00m, "123456789", 1, "Main Branch", 1, "City Pharmacy", new DateTime(2025, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 10, 30, 0, 0), null, null });

            migrationBuilder.InsertData(
                table: "Approvals",
                columns: new[] { "Id", "AdditionalPool", "ChronicForDate", "ClaimFormNumber", "Comment", "CreatedAt", "CreatedBy", "Diagnosis", "EmailOrPhone", "InternalNote", "IsDebit", "IsDelivery", "IsRepeated", "MaxAllowedAmount", "MemberIdentifier", "MemberPolicyInfoId", "ProviderBranch", "ProviderId", "ProviderName", "ReceiveDate", "ReceiveTime", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 2, null, null, "CF-2025-002", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "system", "Hypertension treatment", null, null, false, true, false, 350.00m, "987654321", 1, "Downtown", 1, "Health Pharmacy", new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 14, 15, 0, 0), null, null });

            migrationBuilder.InsertData(
                table: "Approvals",
                columns: new[] { "Id", "AdditionalPool", "ChronicForDate", "ClaimFormNumber", "Comment", "CreatedAt", "CreatedBy", "Diagnosis", "EmailOrPhone", "InternalNote", "IsApproved", "IsDebit", "IsDelivery", "IsDispensed", "IsFromProviderPortal", "IsRepeated", "MaxAllowedAmount", "MemberIdentifier", "MemberPolicyInfoId", "ProviderBranch", "ProviderId", "ProviderName", "ReceiveDate", "ReceiveTime", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 3, null, null, "CF-2025-003", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "system", "Asthma medication", null, null, true, true, false, true, true, true, 450.00m, "555666777", 1, "North Branch", 1, "Care Pharmacy", new DateTime(2025, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 9, 0, 0, 0), null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_MemberIdentifier",
                table: "Approvals",
                column: "MemberIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_MemberPolicyInfos_MemberPolicyInfoId",
                table: "Approvals",
                column: "MemberPolicyInfoId",
                principalTable: "MemberPolicyInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
