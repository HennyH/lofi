using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.Database.Migrations
{
    public partial class RestructureTipAndTipPayoutEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payout_receipts_tip_payouts_tip_payout_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropForeignKey(
                name: "fk_tip_payouts_tips_tip_id",
                table: "tip_payouts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payout_receipts_tip_payout_id",
                table: "tip_payout_receipts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payments_tip_id",
                table: "tip_payments");

            migrationBuilder.DropColumn(
                name: "tip_payout_id",
                table: "tip_payout_receipts");

            migrationBuilder.RenameColumn(
                name: "tip_id",
                table: "tip_payouts",
                newName: "tip_payment_id");

            migrationBuilder.RenameIndex(
                name: "ix_tip_payouts_tip_id_artist_id",
                table: "tip_payouts",
                newName: "ix_tip_payouts_tip_payment_id_artist_id");

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
                name: "ix_tip_payments_tip_id",
                table: "tip_payments",
                column: "tip_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payouts_tip_payments_tip_payment_id",
                table: "tip_payouts",
                column: "tip_payment_id",
                principalTable: "tip_payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payouts_tip_payout_receipts_receipt_id",
                table: "tip_payouts",
                column: "receipt_id",
                principalTable: "tip_payout_receipts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tip_payouts_tip_payments_tip_payment_id",
                table: "tip_payouts");

            migrationBuilder.DropForeignKey(
                name: "fk_tip_payouts_tip_payout_receipts_receipt_id",
                table: "tip_payouts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payouts_receipt_id",
                table: "tip_payouts");

            migrationBuilder.DropIndex(
                name: "ix_tip_payments_tip_id",
                table: "tip_payments");

            migrationBuilder.DropColumn(
                name: "receipt_id",
                table: "tip_payouts");

            migrationBuilder.RenameColumn(
                name: "tip_payment_id",
                table: "tip_payouts",
                newName: "tip_id");

            migrationBuilder.RenameIndex(
                name: "ix_tip_payouts_tip_payment_id_artist_id",
                table: "tip_payouts",
                newName: "ix_tip_payouts_tip_id_artist_id");

            migrationBuilder.AddColumn<int>(
                name: "tip_payout_id",
                table: "tip_payout_receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_tip_payout_receipts_tip_payout_id",
                table: "tip_payout_receipts",
                column: "tip_payout_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tip_payments_tip_id",
                table: "tip_payments",
                column: "tip_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payout_receipts_tip_payouts_tip_payout_id",
                table: "tip_payout_receipts",
                column: "tip_payout_id",
                principalTable: "tip_payouts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tip_payouts_tips_tip_id",
                table: "tip_payouts",
                column: "tip_id",
                principalTable: "tips",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
