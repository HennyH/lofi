using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class AddAmountToTipForPayouts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayedOutDate",
                table: "Tips",
                newName: "PayoutDate");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Tips",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayoutTransactionId",
                table: "Tips",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "PayoutTransactionId",
                table: "Tips");

            migrationBuilder.RenameColumn(
                name: "PayoutDate",
                table: "Tips",
                newName: "PayedOutDate");
        }
    }
}
