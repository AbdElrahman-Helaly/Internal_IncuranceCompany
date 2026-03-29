using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintFromClientActivePolicyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the unique index
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_ActivePolicyId' AND object_id = OBJECT_ID('Clients'))
                BEGIN
                    DROP INDEX [IX_Clients_ActivePolicyId] ON [Clients];
                END
            ");

            // Create a non-unique index instead
            migrationBuilder.CreateIndex(
                name: "IX_Clients_ActivePolicyId",
                table: "Clients",
                column: "ActivePolicyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the non-unique index
            migrationBuilder.DropIndex(
                name: "IX_Clients_ActivePolicyId",
                table: "Clients");

            // Recreate as unique (original state)
            migrationBuilder.CreateIndex(
                name: "IX_Clients_ActivePolicyId",
                table: "Clients",
                column: "ActivePolicyId",
                unique: true,
                filter: "[ActivePolicyId] IS NOT NULL");
        }
    }
}

