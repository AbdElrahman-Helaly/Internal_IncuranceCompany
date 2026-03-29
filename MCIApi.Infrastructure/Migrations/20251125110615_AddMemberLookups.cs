using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "ClientMemberSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LevelName",
                table: "ClientMemberSnapshots",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VipStatusId",
                table: "ClientMemberSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VipStatusName",
                table: "ClientMemberSnapshots",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MemberLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VipStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_LevelId",
                table: "ClientMemberSnapshots",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_VipStatusId",
                table: "ClientMemberSnapshots",
                column: "VipStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMemberSnapshots_MemberLevels_LevelId",
                table: "ClientMemberSnapshots",
                column: "LevelId",
                principalTable: "MemberLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMemberSnapshots_VipStatuses_VipStatusId",
                table: "ClientMemberSnapshots",
                column: "VipStatusId",
                principalTable: "VipStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientMemberSnapshots_MemberLevels_LevelId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientMemberSnapshots_VipStatuses_VipStatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropTable(
                name: "MemberLevels");

            migrationBuilder.DropTable(
                name: "VipStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ClientMemberSnapshots_LevelId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_ClientMemberSnapshots_VipStatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "LevelName",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "VipStatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "VipStatusName",
                table: "ClientMemberSnapshots");
        }
    }
}
