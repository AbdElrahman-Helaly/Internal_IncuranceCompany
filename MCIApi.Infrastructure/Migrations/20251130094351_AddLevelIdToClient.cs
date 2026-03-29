using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelIdToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "LevelId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_LevelId",
                table: "Clients",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_MemberLevels_LevelId",
                table: "Clients",
                column: "LevelId",
                principalTable: "MemberLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_MemberLevels_LevelId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_LevelId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Clients");
        }
    }
}
