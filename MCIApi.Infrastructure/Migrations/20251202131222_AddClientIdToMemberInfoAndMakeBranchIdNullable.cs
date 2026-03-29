using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientIdToMemberInfoAndMakeBranchIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Make BranchId nullable
            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "MemberInfos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Step 2: Add ClientId as nullable first
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "MemberInfos",
                type: "int",
                nullable: true);

            // Step 3: Populate ClientId from Branch.ClientId for existing records
            migrationBuilder.Sql(@"
                UPDATE m
                SET m.ClientId = b.ClientId
                FROM MemberInfos m
                INNER JOIN Branches b ON m.BranchId = b.Id
                WHERE m.ClientId IS NULL AND m.BranchId IS NOT NULL
            ");

            // Step 4: For members without a branch, try to find client from MemberPolicyInfo
            migrationBuilder.Sql(@"
                UPDATE m
                SET m.ClientId = p.ClientId
                FROM MemberInfos m
                INNER JOIN MemberPolicyInfos mpi ON m.Id = mpi.MemberId
                INNER JOIN Policies p ON mpi.PolicyId = p.Id
                WHERE m.ClientId IS NULL AND p.ClientId IS NOT NULL
            ");

            // Step 5: For any remaining members without ClientId, set to a default (or handle as needed)
            // Note: This should not happen in production, but we need to handle it for migration
            migrationBuilder.Sql(@"
                UPDATE MemberInfos
                SET ClientId = (SELECT TOP 1 Id FROM Clients WHERE IsDeleted = 0 ORDER BY Id)
                WHERE ClientId IS NULL
            ");

            // Step 6: Make ClientId non-nullable
            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "MemberInfos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Step 7: Create index and foreign key
            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_ClientId",
                table: "MemberInfos",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInfos_Clients_ClientId",
                table: "MemberInfos",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberInfos_Clients_ClientId",
                table: "MemberInfos");

            migrationBuilder.DropIndex(
                name: "IX_MemberInfos_ClientId",
                table: "MemberInfos");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "MemberInfos");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "MemberInfos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
