using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class AddTipPayoutAndPaymentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipPayment_Tips_TipId",
                table: "TipPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_TipPayout_Artists_ArtistId",
                table: "TipPayout");

            migrationBuilder.DropForeignKey(
                name: "FK_TipPayout_Tips_TipId",
                table: "TipPayout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipPayout",
                table: "TipPayout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipPayment",
                table: "TipPayment");

            migrationBuilder.RenameTable(
                name: "TipPayout",
                newName: "TipPayouts");

            migrationBuilder.RenameTable(
                name: "TipPayment",
                newName: "TipPayments");

            migrationBuilder.RenameIndex(
                name: "IX_TipPayout_ArtistId",
                table: "TipPayouts",
                newName: "IX_TipPayouts_ArtistId");

            migrationBuilder.RenameIndex(
                name: "IX_TipPayment_TipId",
                table: "TipPayments",
                newName: "IX_TipPayments_TipId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipPayouts",
                table: "TipPayouts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipPayments",
                table: "TipPayments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TipPayments_Tips_TipId",
                table: "TipPayments",
                column: "TipId",
                principalTable: "Tips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TipPayouts_Artists_ArtistId",
                table: "TipPayouts",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TipPayouts_Tips_TipId",
                table: "TipPayouts",
                column: "TipId",
                principalTable: "Tips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipPayments_Tips_TipId",
                table: "TipPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_TipPayouts_Artists_ArtistId",
                table: "TipPayouts");

            migrationBuilder.DropForeignKey(
                name: "FK_TipPayouts_Tips_TipId",
                table: "TipPayouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipPayouts",
                table: "TipPayouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipPayments",
                table: "TipPayments");

            migrationBuilder.RenameTable(
                name: "TipPayouts",
                newName: "TipPayout");

            migrationBuilder.RenameTable(
                name: "TipPayments",
                newName: "TipPayment");

            migrationBuilder.RenameIndex(
                name: "IX_TipPayouts_ArtistId",
                table: "TipPayout",
                newName: "IX_TipPayout_ArtistId");

            migrationBuilder.RenameIndex(
                name: "IX_TipPayments_TipId",
                table: "TipPayment",
                newName: "IX_TipPayment_TipId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipPayout",
                table: "TipPayout",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipPayment",
                table: "TipPayment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TipPayment_Tips_TipId",
                table: "TipPayment",
                column: "TipId",
                principalTable: "Tips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TipPayout_Artists_ArtistId",
                table: "TipPayout",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TipPayout_Tips_TipId",
                table: "TipPayout",
                column: "TipId",
                principalTable: "Tips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
