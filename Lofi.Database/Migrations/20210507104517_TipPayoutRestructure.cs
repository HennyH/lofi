using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.Database.Migrations
{
    public partial class TipPayoutRestructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payout_receipts_transfers_payout_transfer_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payout_receipts_payout_transfer_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropColumn(
                name: "payout_transfer_id",
                table: "tip_payout_receipts");

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "tip_payout_receipts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "tip_payout_receipts");

            migrationBuilder.AddColumn<int>(
                name: "payout_transfer_id",
                table: "tip_payout_receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_tip_payout_receipts_payout_transfer_id",
                table: "tip_payout_receipts",
                column: "payout_transfer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payout_receipts_transfers_payout_transfer_id",
                table: "tip_payout_receipts",
                column: "payout_transfer_id",
                principalTable: "transfers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
