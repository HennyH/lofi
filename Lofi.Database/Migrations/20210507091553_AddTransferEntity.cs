using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Lofi.Database.Migrations
{
    public partial class AddTransferEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "block_height",
                table: "tip_payout_receipts");

            migrationBuilder.DropColumn(
                name: "payout_timestamp",
                table: "tip_payout_receipts");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropColumn(
                name: "wallet_address",
                table: "tip_payout_receipts");

            migrationBuilder.DropColumn(
                name: "amount",
                table: "tip_payments");

            migrationBuilder.DropColumn(
                name: "block_height",
                table: "tip_payments");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "tip_payments");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "tip_payments");

            migrationBuilder.AddColumn<int>(
                name: "payout_transfer_id",
                table: "tip_payout_receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "payment_transfer_id",
                table: "tip_payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "transfers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    transaction_id = table.Column<string>(type: "text", nullable: false),
                    from_wallet_address = table.Column<string>(type: "text", nullable: false),
                    to_wallet_address = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    transaction_fee = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    payment_id = table.Column<int>(type: "integer", nullable: false),
                    block_height = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    timestamp = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transfers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tip_payout_receipts_payout_transfer_id",
                table: "tip_payout_receipts",
                column: "payout_transfer_id");

            migrationBuilder.CreateIndex(
                name: "ix_tip_payments_payment_transfer_id",
                table: "tip_payments",
                column: "payment_transfer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payments_transfers_payment_transfer_id",
                table: "tip_payments",
                column: "payment_transfer_id",
                principalTable: "transfers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payout_receipts_transfers_payout_transfer_id",
                table: "tip_payout_receipts",
                column: "payout_transfer_id",
                principalTable: "transfers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payments_transfers_payment_transfer_id",
                table: "tip_payments");

            migrationBuilder.DropForeignKey(
                name: "fk_tip_payout_receipts_transfers_payout_transfer_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropTable(
                name: "transfers");

            migrationBuilder.DropIndex(
                name: "ix_tip_payout_receipts_payout_transfer_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payments_payment_transfer_id",
                table: "tip_payments");

            migrationBuilder.DropColumn(
                name: "payout_transfer_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropColumn(
                name: "payment_transfer_id",
                table: "tip_payments");

            migrationBuilder.AddColumn<decimal>(
                name: "block_height",
                table: "tip_payout_receipts",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "payout_timestamp",
                table: "tip_payout_receipts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "tip_payout_receipts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "wallet_address",
                table: "tip_payout_receipts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                table: "tip_payments",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "block_height",
                table: "tip_payments",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "timestamp",
                table: "tip_payments",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "tip_payments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
