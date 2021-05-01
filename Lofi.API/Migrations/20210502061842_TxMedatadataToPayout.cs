using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class TxMedatadataToPayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_artist_tip_payouts_tip_payments_payout_id",
                table: "artist_tip_payouts");

            migrationBuilder.AddColumn<decimal>(
                name: "block_height",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "timestamp",
                table: "tip_payouts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "tip_payouts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_artist_tip_payouts_tip_payouts_payout_id",
                table: "artist_tip_payouts",
                column: "payout_id",
                principalTable: "tip_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_artist_tip_payouts_tip_payouts_payout_id",
                table: "artist_tip_payouts");

            migrationBuilder.DropColumn(
                name: "block_height",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "tip_payouts");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "tip_payouts");

            migrationBuilder.AddForeignKey(
                name: "fk_artist_tip_payouts_tip_payments_payout_id",
                table: "artist_tip_payouts",
                column: "payout_id",
                principalTable: "tip_payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
