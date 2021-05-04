using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Lofi.Database.Migrations
{
    public partial class AddTipPayourReceipts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "block_height",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "net_payout_amount",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "payout_date",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "payout_timestamp",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "payout_tx_fee",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "payout_tx_fee_share",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "wallet_address",
                table: "tip_payouts");

            migrationBuilder.RenameColumn(
                name: "gross_payout_amount",
                table: "tip_payouts",
                newName: "amount");

            migrationBuilder.AddColumn<int>(
                name: "receipt_id",
                table: "tip_payouts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tip_payout_receipt",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tip_id = table.Column<int>(type: "integer", nullable: false),
                    net_payout_amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    payout_tx_fee = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    payout_tx_fee_share = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    transaction_id = table.Column<string>(type: "text", nullable: false),
                    block_height = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    wallet_address = table.Column<string>(type: "text", nullable: false),
                    payout_timestamp = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tip_payout_receipt", x => x.id);
                    table.ForeignKey(
                        name: "fk_tip_payout_receipt_tips_tip_id",
                        column: x => x.tip_id,
                        principalTable: "tips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tip_payouts_receipt_id",
                table: "tip_payouts",
                column: "receipt_id");

            migrationBuilder.CreateIndex(
                name: "ix_tip_payout_receipt_tip_id",
                table: "tip_payout_receipt",
                column: "tip_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payouts_tip_payout_receipt_receipt_id",
                table: "tip_payouts",
                column: "receipt_id",
                principalTable: "tip_payout_receipt",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payouts_tip_payout_receipt_receipt_id",
                table: "tip_payouts");

            migrationBuilder.DropTable(
                name: "tip_payout_receipt");

            migrationBuilder.DropIndex(
                name: "ix_tip_payouts_receipt_id",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "receipt_id",
                table: "tip_payouts");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "tip_payouts",
                newName: "gross_payout_amount");

            migrationBuilder.AddColumn<decimal>(
                name: "block_height",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "net_payout_amount",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true);

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

            migrationBuilder.AddColumn<decimal>(
                name: "payout_tx_fee",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "payout_tx_fee_share",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "tip_payouts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wallet_address",
                table: "tip_payouts",
                type: "text",
                nullable: true);
        }
    }
}
