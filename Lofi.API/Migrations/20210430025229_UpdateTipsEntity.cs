using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class UpdateTipsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaymentConfirmed",
                table: "Tips");

            migrationBuilder.RenameColumn(
                name: "TipDate",
                table: "Tips",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "PayedOutDate",
                table: "Tips",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Tips",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIdHex",
                table: "Tips",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionConfirmedDate",
                table: "Tips",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayedOutDate",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "PaymentIdHex",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "TransactionConfirmedDate",
                table: "Tips");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Tips",
                newName: "TipDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaymentConfirmed",
                table: "Tips",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
