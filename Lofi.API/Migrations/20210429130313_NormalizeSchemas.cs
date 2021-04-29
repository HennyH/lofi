using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class NormalizeSchemas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UploadDate",
                table: "Albums",
                newName: "ModifiedDate");

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "Files",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "Artists",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Albums",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Artists_ProfilePictureId",
                table: "Artists",
                column: "ProfilePictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Files_ProfilePictureId",
                table: "Artists",
                column: "ProfilePictureId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Files_ProfilePictureId",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists_ProfilePictureId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Albums");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Albums",
                newName: "UploadDate");

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
