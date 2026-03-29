using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Consumptions",
                table: "ClientMemberSnapshots");

            migrationBuilder.RenameColumn(
                name: "Program",
                table: "ClientMemberSnapshots",
                newName: "ProgramName");

            migrationBuilder.RenameColumn(
                name: "ChangeStatus",
                table: "ClientMemberSnapshots",
                newName: "JobTitle");

            migrationBuilder.AddColumn<int>(
                name: "ClientStatusId",
                table: "ClientMemberSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "ClientMemberSnapshots",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HofCode",
                table: "ClientMemberSnapshots",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ClientMemberSnapshots",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMale",
                table: "ClientMemberSnapshots",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "ClientMemberSnapshots",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "ClientMemberSnapshots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_ClientStatusId",
                table: "ClientMemberSnapshots",
                column: "ClientStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberSnapshots_ProgramId",
                table: "ClientMemberSnapshots",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMemberSnapshots_ClientStatuses_ClientStatusId",
                table: "ClientMemberSnapshots",
                column: "ClientStatusId",
                principalTable: "ClientStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMemberSnapshots_Programs_ProgramId",
                table: "ClientMemberSnapshots",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientMemberSnapshots_ClientStatuses_ClientStatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientMemberSnapshots_Programs_ProgramId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_ClientMemberSnapshots_ClientStatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_ClientMemberSnapshots_ProgramId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "ClientStatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "HofCode",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "IsMale",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "ClientMemberSnapshots");

            migrationBuilder.RenameColumn(
                name: "ProgramName",
                table: "ClientMemberSnapshots",
                newName: "Program");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                table: "ClientMemberSnapshots",
                newName: "ChangeStatus");

            migrationBuilder.AddColumn<decimal>(
                name: "Consumptions",
                table: "ClientMemberSnapshots",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
