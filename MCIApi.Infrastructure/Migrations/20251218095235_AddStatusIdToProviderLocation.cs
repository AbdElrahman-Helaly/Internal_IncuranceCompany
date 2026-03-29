using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusIdToProviderLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "ProviderLocations",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "ProviderLocations",
                keyColumn: "Id",
                keyValue: 1,
                column: "StatusId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderLocations_StatusId",
                table: "ProviderLocations",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderLocations_Statuses_StatusId",
                table: "ProviderLocations",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderLocations_Statuses_StatusId",
                table: "ProviderLocations");

            migrationBuilder.DropIndex(
                name: "IX_ProviderLocations_StatusId",
                table: "ProviderLocations");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "ProviderLocations");
        }
    }
}
