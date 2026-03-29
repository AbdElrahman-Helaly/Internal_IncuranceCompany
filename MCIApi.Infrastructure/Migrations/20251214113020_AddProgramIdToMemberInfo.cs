using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProgramIdToMemberInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReimbursementTypeId",
                table: "RefundRules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "MemberInfos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReimbursementPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReimbursementPrograms", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "MemberInfos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProgramId",
                value: null);

            migrationBuilder.InsertData(
                table: "ReimbursementPrograms",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EndDate", "NameAr", "NameEn", "StartDate", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د مصطفی احمد حمدی", "Dr. Mostafa Ahmed Hamdy", new DateTime(2022, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "سموحة للاشعه", "Samouha for Radiology", new DateTime(2021, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د/ وليد محمد اسماعيل", "Dr. Waleed Mohamed Ismail", new DateTime(2022, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د/ مجدي هنري ساويرس", "Dr. Magdy Henry Sawiris", new DateTime(2022, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د ماجد حشمت ابراهیم", "Dr. Majed Hashemet Ibrahim", new DateTime(2022, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(3000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "د محمد مامون", "Dr. Mohamed Mamoun", new DateTime(2022, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Seeder", new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ل خالد لطفي عبد الحليم", "Dr. Khaled Lotfy Abdel Halim", new DateTime(2022, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_ProgramId",
                table: "MemberInfos",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInfos_Programs_ProgramId",
                table: "MemberInfos",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberInfos_Programs_ProgramId",
                table: "MemberInfos");

            migrationBuilder.DropTable(
                name: "ReimbursementPrograms");

            migrationBuilder.DropIndex(
                name: "IX_MemberInfos_ProgramId",
                table: "MemberInfos");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "MemberInfos");

            migrationBuilder.AlterColumn<int>(
                name: "ReimbursementTypeId",
                table: "RefundRules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
