using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class AddPayoutAmountToAdjustForFeesToTips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PayoutAmount",
                table: "Tips",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Artists",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Artists",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayoutAmount",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Artists");
        }
    }
}
