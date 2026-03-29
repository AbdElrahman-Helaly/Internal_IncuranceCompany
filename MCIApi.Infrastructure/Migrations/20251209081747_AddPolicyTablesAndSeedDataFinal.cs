using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyTablesAndSeedDataFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomClass",
                table: "GeneralPrograms");

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeId",
                table: "GeneralPrograms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomTypes",
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
                    table.PrimaryKey("PK_RoomTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "NameAr", "NameEn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "جناح", "Suit", null, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "جناح صغير", "Mini Suit", null, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة أولى فردية", "First Class Single", null, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة أولى مزدوجة", "First Class Double", null, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة ثانية فردية", "Second Class Single", null, null },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة ثانية مزدوجة", "Second Class Double", null, null },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seeder", "غرفة ثانية ثلاثية", "Second Class Triple", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneralPrograms_RoomTypeId",
                table: "GeneralPrograms",
                column: "RoomTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralPrograms_RoomTypes_RoomTypeId",
                table: "GeneralPrograms",
                column: "RoomTypeId",
                principalTable: "RoomTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralPrograms_RoomTypes_RoomTypeId",
                table: "GeneralPrograms");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropIndex(
                name: "IX_GeneralPrograms_RoomTypeId",
                table: "GeneralPrograms");

            migrationBuilder.DropColumn(
                name: "RoomTypeId",
                table: "GeneralPrograms");

            migrationBuilder.AddColumn<string>(
                name: "RoomClass",
                table: "GeneralPrograms",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
