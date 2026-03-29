using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCIApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClaimServiceClassToMatchApprovalServiceClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoicePrice",
                table: "ClaimServiceClasses");

            migrationBuilder.DropColumn(
                name: "MoneyPaidByMember",
                table: "ClaimServiceClasses");

            migrationBuilder.DropColumn(
                name: "RequestedPrice",
                table: "ClaimServiceClasses");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "ClaimServiceClasses");

            migrationBuilder.AddColumn<int>(
                name: "CtoNameId",
                table: "ClaimServiceClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Copayment",
                table: "ClaimServiceClasses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ReasonId",
                table: "ClaimServiceClasses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "ClaimServiceClasses",
                type: "int",
                nullable: false,
                defaultValue: 1); // Default to APPROVED = 1
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CtoNameId",
                table: "ClaimServiceClasses");

            migrationBuilder.DropColumn(
                name: "Copayment",
                table: "ClaimServiceClasses");

            migrationBuilder.DropColumn(
                name: "ReasonId",
                table: "ClaimServiceClasses");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "ClaimServiceClasses");

            migrationBuilder.AddColumn<decimal>(
                name: "InvoicePrice",
                table: "ClaimServiceClasses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MoneyPaidByMember",
                table: "ClaimServiceClasses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedPrice",
                table: "ClaimServiceClasses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "ClaimServiceClasses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
