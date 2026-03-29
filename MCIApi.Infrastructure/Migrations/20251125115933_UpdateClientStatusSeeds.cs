using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientStatusSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Note: This migration is now redundant as the initial seed data in AddClientCreateAggregates
            // has been corrected to match the final values. The data will be recreated correctly
            // in the RenameClientStatusToStatus migration when the table is renamed to Statuses.
            // Keeping this migration as a no-op to maintain migration history consistency.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: See Up method comment
        }
    }
}
