using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameClientStatusToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_ClientStatuses_BranchStatusId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientMemberSnapshots_ClientStatuses_ClientStatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ClientStatuses_StatusId",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "ClientStatuses");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ClientMemberSnapshots",
                newName: "StatusName");

            migrationBuilder.RenameColumn(
                name: "ClientStatusId",
                table: "ClientMemberSnapshots",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMemberSnapshots_ClientStatusId",
                table: "ClientMemberSnapshots",
                newName: "IX_ClientMemberSnapshots_StatusId");

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "مفعل", "Activated" },
                    { 2, "غير مفعل", "Deactivated" },
                    { 3, "معلق", "Hold" },
                    { 4, "قيد الانتظار", "Pending" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Statuses_BranchStatusId",
                table: "Branches",
                column: "BranchStatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMemberSnapshots_Statuses_StatusId",
                table: "ClientMemberSnapshots",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Statuses_StatusId",
                table: "Clients",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Statuses_BranchStatusId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientMemberSnapshots_Statuses_StatusId",
                table: "ClientMemberSnapshots");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Statuses_StatusId",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.RenameColumn(
                name: "StatusName",
                table: "ClientMemberSnapshots",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "ClientMemberSnapshots",
                newName: "ClientStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientMemberSnapshots_StatusId",
                table: "ClientMemberSnapshots",
                newName: "IX_ClientMemberSnapshots_ClientStatusId");

            migrationBuilder.CreateTable(
                name: "ClientStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ClientStatuses",
                columns: new[] { "Id", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { 1, "مفعل", "Activated" },
                    { 2, "غير مفعل", "Deactivated" },
                    { 3, "معلق", "Hold" },
                    { 4, "قيد الانتظار", "Pending" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_ClientStatuses_BranchStatusId",
                table: "Branches",
                column: "BranchStatusId",
                principalTable: "ClientStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMemberSnapshots_ClientStatuses_ClientStatusId",
                table: "ClientMemberSnapshots",
                column: "ClientStatusId",
                principalTable: "ClientStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_ClientStatuses_StatusId",
                table: "Clients",
                column: "StatusId",
                principalTable: "ClientStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
