using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class RenameFilesTableToUploadedFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_Files_CoverPhotoFileId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Files_ProfilePictureId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Files_AudioFileId",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Files_CoverPhotoFileId",
                table: "Tracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "UploadedFiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UploadedFiles",
                table: "UploadedFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_UploadedFiles_CoverPhotoFileId",
                table: "Albums",
                column: "CoverPhotoFileId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_UploadedFiles_ProfilePictureId",
                table: "Artists",
                column: "ProfilePictureId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_UploadedFiles_AudioFileId",
                table: "Tracks",
                column: "AudioFileId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_UploadedFiles_CoverPhotoFileId",
                table: "Tracks",
                column: "CoverPhotoFileId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_UploadedFiles_CoverPhotoFileId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Artists_UploadedFiles_ProfilePictureId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_UploadedFiles_AudioFileId",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_UploadedFiles_CoverPhotoFileId",
                table: "Tracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UploadedFiles",
                table: "UploadedFiles");

            migrationBuilder.RenameTable(
                name: "UploadedFiles",
                newName: "Files");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_Files_CoverPhotoFileId",
                table: "Albums",
                column: "CoverPhotoFileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Files_ProfilePictureId",
                table: "Artists",
                column: "ProfilePictureId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Files_AudioFileId",
                table: "Tracks",
                column: "AudioFileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Files_CoverPhotoFileId",
                table: "Tracks",
                column: "CoverPhotoFileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
