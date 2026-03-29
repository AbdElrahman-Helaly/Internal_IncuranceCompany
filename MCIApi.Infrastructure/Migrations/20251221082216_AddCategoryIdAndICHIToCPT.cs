using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdAndICHIToCPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CPTs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ICHI",
                table: "CPTs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CPTs_CategoryId",
                table: "CPTs",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CPTs_Categories_CategoryId",
                table: "CPTs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CPTs_Categories_CategoryId",
                table: "CPTs");

            migrationBuilder.DropIndex(
                name: "IX_CPTs_CategoryId",
                table: "CPTs");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CPTs");

            migrationBuilder.DropColumn(
                name: "ICHI",
                table: "CPTs");
        }
    }
}
