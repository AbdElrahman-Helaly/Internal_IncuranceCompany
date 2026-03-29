using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMemberInfoSeedDataWithClientId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MemberInfos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClientId",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MemberInfos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClientId",
                value: 0);
        }
    }
}
