using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeparateChronicAndRegularApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsChronic to Approvals
            migrationBuilder.AddColumn<bool>(
                name: "IsChronic",
                table: "Approvals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Add new columns to ApprovalServiceClasses
            migrationBuilder.AddColumn<int>(
                name: "CtoNameId",
                table: "ApprovalServiceClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ApprovalServiceClasses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Qty",
                table: "ApprovalServiceClasses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "ApprovalServiceClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReasonId",
                table: "ApprovalServiceClasses",
                type: "int",
                nullable: true);

            // Add new columns to ApprovalMedicines
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "ApprovalMedicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Days",
                table: "ApprovalMedicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CP",
                table: "ApprovalMedicines",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "ApprovalMedicines",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChronic",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "CtoNameId",
                table: "ApprovalServiceClasses");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ApprovalServiceClasses");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "ApprovalServiceClasses");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "ApprovalServiceClasses");

            migrationBuilder.DropColumn(
                name: "ReasonId",
                table: "ApprovalServiceClasses");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ApprovalMedicines");

            migrationBuilder.DropColumn(
                name: "Days",
                table: "ApprovalMedicines");

            migrationBuilder.DropColumn(
                name: "CP",
                table: "ApprovalMedicines");

            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "ApprovalMedicines");
        }
    }
}
