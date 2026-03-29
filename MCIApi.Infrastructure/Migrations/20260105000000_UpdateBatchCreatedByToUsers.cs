using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBatchCreatedByToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop old foreign key constraints (only if they exist)
            var sql = @"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Batches_Employees_CreatedBy')
                    ALTER TABLE [Batches] DROP CONSTRAINT [FK_Batches_Employees_CreatedBy];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Batches_Employees_UpdatedBy')
                    ALTER TABLE [Batches] DROP CONSTRAINT [FK_Batches_Employees_UpdatedBy];
            ";
            migrationBuilder.Sql(sql);

            // Step 2: Drop indexes that depend on CreatedBy and UpdatedBy columns (only if they exist)
            var dropIndexSql = @"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Batches_CreatedBy' AND object_id = OBJECT_ID('Batches'))
                    DROP INDEX [IX_Batches_CreatedBy] ON [Batches];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Batches_UpdatedBy' AND object_id = OBJECT_ID('Batches'))
                    DROP INDEX [IX_Batches_UpdatedBy] ON [Batches];
            ";
            migrationBuilder.Sql(dropIndexSql);

            // Step 3: Change CreatedBy column from int to nvarchar(450)
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Batches",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Step 4: Change UpdatedBy column from int? to nvarchar(450)
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Batches",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Step 5: Add new foreign key constraints linking to AspNetUsers
            migrationBuilder.AddForeignKey(
                name: "FK_Batches_AspNetUsers_CreatedBy",
                table: "Batches",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_AspNetUsers_UpdatedBy",
                table: "Batches",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_AspNetUsers_CreatedBy",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_Batches_AspNetUsers_UpdatedBy",
                table: "Batches");

            // Revert CreatedBy column back to int
            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Batches",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            // Revert UpdatedBy column back to int?
            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "Batches",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            // Restore indexes (if they existed)
            migrationBuilder.CreateIndex(
                name: "IX_Batches_CreatedBy",
                table: "Batches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_UpdatedBy",
                table: "Batches",
                column: "UpdatedBy");

            // Restore old foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Employees_CreatedBy",
                table: "Batches",
                column: "CreatedBy",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Employees_UpdatedBy",
                table: "Batches",
                column: "UpdatedBy",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

