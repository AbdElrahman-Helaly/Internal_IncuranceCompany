using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalServicesRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Medicines");

            migrationBuilder.AddColumn<int>(
                name: "PoolId",
                table: "Approvals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApprovalServiceClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalId = table.Column<int>(type: "int", nullable: false),
                    ServiceClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalServiceClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalServiceClasses_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalServiceClasses_ServiceClasses_ServiceClassId",
                        column: x => x.ServiceClassId,
                        principalTable: "ServiceClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_PoolId",
                table: "Approvals",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalServiceClasses_ApprovalId_ServiceClassId",
                table: "ApprovalServiceClasses",
                columns: new[] { "ApprovalId", "ServiceClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalServiceClasses_ServiceClassId",
                table: "ApprovalServiceClasses",
                column: "ServiceClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Pools_PoolId",
                table: "Approvals",
                column: "PoolId",
                principalTable: "Pools",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Pools_PoolId",
                table: "Approvals");

            migrationBuilder.DropTable(
                name: "ApprovalServiceClasses");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_PoolId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "PoolId",
                table: "Approvals");

            migrationBuilder.AddColumn<string>(
                name: "IsActive",
                table: "Medicines",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
