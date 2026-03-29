using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInpatientDurationAndDurationTypeToApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InpatientDuration",
                table: "Approvals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationType",
                table: "Approvals",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InpatientDuration",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "DurationType",
                table: "Approvals");
        }
    }
}

