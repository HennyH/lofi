using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class AddBlockHeightToTip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionConfirmedDate",
                table: "Tips");

            migrationBuilder.RenameColumn(
                name: "TransactionHash",
                table: "Tips",
                newName: "TransactionId");

            migrationBuilder.AlterColumn<string>(
                name: "IntegratedPaymentAddress",
                table: "Tips",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BlockHeight",
                table: "Tips",
                type: "numeric(20,0)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockHeight",
                table: "Tips");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Tips",
                newName: "TransactionHash");

            migrationBuilder.AlterColumn<string>(
                name: "IntegratedPaymentAddress",
                table: "Tips",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionConfirmedDate",
                table: "Tips",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}
