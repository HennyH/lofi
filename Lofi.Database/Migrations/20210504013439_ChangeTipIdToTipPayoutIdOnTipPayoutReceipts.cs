using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.Database.Migrations
{
    public partial class ChangeTipIdToTipPayoutIdOnTipPayoutReceipts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payout_receipt_tips_tip_id",
                table: "tip_payout_receipt");

            migrationBuilder.DropForeignKey(
                name: "fk_tip_payouts_tip_payout_receipt_receipt_id",
                table: "tip_payouts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payouts_receipt_id",
                table: "tip_payouts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payout_receipt_tip_id",
                table: "tip_payout_receipt");

            migrationBuilder.DropColumn(
                name: "receipt_id",
                table: "tip_payouts");

            migrationBuilder.RenameColumn(
                name: "tip_id",
                table: "tip_payout_receipt",
                newName: "tip_payout_id");

            migrationBuilder.CreateIndex(
                name: "ix_tip_payout_receipt_tip_payout_id",
                table: "tip_payout_receipt",
                column: "tip_payout_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payout_receipt_tip_payouts_tip_payout_id",
                table: "tip_payout_receipt",
                column: "tip_payout_id",
                principalTable: "tip_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payout_receipt_tip_payouts_tip_payout_id",
                table: "tip_payout_receipt");

            migrationBuilder.DropIndex(
                name: "ix_tip_payout_receipt_tip_payout_id",
                table: "tip_payout_receipt");

            migrationBuilder.RenameColumn(
                name: "tip_payout_id",
                table: "tip_payout_receipt",
                newName: "tip_id");

            migrationBuilder.AddColumn<int>(
                name: "receipt_id",
                table: "tip_payouts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tip_payouts_receipt_id",
                table: "tip_payouts",
                column: "receipt_id");

            migrationBuilder.CreateIndex(
                name: "ix_tip_payout_receipt_tip_id",
                table: "tip_payout_receipt",
                column: "tip_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payout_receipt_tips_tip_id",
                table: "tip_payout_receipt",
                column: "tip_id",
                principalTable: "tips",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payouts_tip_payout_receipt_receipt_id",
                table: "tip_payouts",
                column: "receipt_id",
                principalTable: "tip_payout_receipt",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
