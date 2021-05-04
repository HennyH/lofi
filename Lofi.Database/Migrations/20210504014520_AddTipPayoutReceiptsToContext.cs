using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.Database.Migrations
{
    public partial class AddTipPayoutReceiptsToContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payout_receipt_tip_payouts_tip_payout_id",
                table: "tip_payout_receipt");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tip_payout_receipt",
                table: "tip_payout_receipt");

            migrationBuilder.RenameTable(
                name: "tip_payout_receipt",
                newName: "tip_payout_receipts");

            migrationBuilder.RenameIndex(
                name: "ix_tip_payout_receipt_tip_payout_id",
                table: "tip_payout_receipts",
                newName: "ix_tip_payout_receipts_tip_payout_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tip_payout_receipts",
                table: "tip_payout_receipts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payout_receipts_tip_payouts_tip_payout_id",
                table: "tip_payout_receipts",
                column: "tip_payout_id",
                principalTable: "tip_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payout_receipts_tip_payouts_tip_payout_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tip_payout_receipts",
                table: "tip_payout_receipts");

            migrationBuilder.RenameTable(
                name: "tip_payout_receipts",
                newName: "tip_payout_receipt");

            migrationBuilder.RenameIndex(
                name: "ix_tip_payout_receipts_tip_payout_id",
                table: "tip_payout_receipt",
                newName: "ix_tip_payout_receipt_tip_payout_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tip_payout_receipt",
                table: "tip_payout_receipt",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payout_receipt_tip_payouts_tip_payout_id",
                table: "tip_payout_receipt",
                column: "tip_payout_id",
                principalTable: "tip_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
