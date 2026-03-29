using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixProviderLocationCascadePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_ProviderLocations_ProviderLocationId",
                table: "Approvals");

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
                name: "FK_Approvals_ProviderLocations_ProviderLocationId",
                table: "Approvals");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_ProviderLocations_ProviderLocationId",
                table: "Approvals",
                column: "ProviderLocationId",
                principalTable: "ProviderLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
