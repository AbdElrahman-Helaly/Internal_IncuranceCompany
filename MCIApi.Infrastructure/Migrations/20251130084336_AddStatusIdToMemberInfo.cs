using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusIdToMemberInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add StatusId column as nullable first
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "MemberInfos",
                type: "int",
                nullable: true);

            // Migrate data from Status string to StatusId int
            // "Active" -> 1, "Deactive" -> 2, default to 1
            migrationBuilder.Sql(@"
                UPDATE MemberInfos 
                SET StatusId = CASE 
                    WHEN Status = 'Active' THEN 1
                    WHEN Status = 'Deactive' THEN 2
                    ELSE 1
                END
            ");

            // Make StatusId not null with default value
            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "MemberInfos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Drop the old Status column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MemberInfos");

            // Create index
            migrationBuilder.CreateIndex(
                name: "IX_MemberInfos_StatusId",
                table: "MemberInfos",
                column: "StatusId");

            // Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_MemberInfos_Statuses_StatusId",
                table: "MemberInfos",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberInfos_Statuses_StatusId",
                table: "MemberInfos");

            migrationBuilder.DropIndex(
                name: "IX_MemberInfos_StatusId",
                table: "MemberInfos");

            // Add Status column back
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MemberInfos",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            // Migrate data back from StatusId to Status
            migrationBuilder.Sql(@"
                UPDATE MemberInfos 
                SET Status = CASE 
                    WHEN StatusId = 1 THEN 'Active'
                    WHEN StatusId = 2 THEN 'Deactive'
                    ELSE 'Active'
                END
            ");

            // Make Status not null
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MemberInfos",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "Active");

            // Drop StatusId column
            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "MemberInfos");
        }
    }
}
