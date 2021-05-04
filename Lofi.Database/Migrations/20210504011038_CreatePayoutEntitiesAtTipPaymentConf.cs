using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.Database.Migrations
{
    public partial class CreatePayoutEntitiesAtTipPaymentConf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "tip_payouts");

            migrationBuilder.AlterColumn<decimal>(
                name: "payout_tx_fee_share",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "payout_tx_fee",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<decimal>(
                name: "net_payout_amount",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddColumn<DateTime>(
                name: "payout_date",
                table: "tip_payouts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "payout_timestamp",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payout_date",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "payout_timestamp",
                table: "tip_payouts");

            migrationBuilder.AlterColumn<decimal>(
                name: "payout_tx_fee_share",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "payout_tx_fee",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "net_payout_amount",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "timestamp",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
