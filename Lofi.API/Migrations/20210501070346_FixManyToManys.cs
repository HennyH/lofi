using Microsoft.EntityFrameworkCore.Migrations;

namespace Lofi.API.Migrations
{
    public partial class FixManyToManys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Albums_AlbumId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Tracks_TrackId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Albums_AlbumId",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Tracks_TrackId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_AlbumId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_TrackId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Artists_AlbumId",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists_TrackId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Artists");

            migrationBuilder.CreateTable(
                name: "AlbumArtist",
                columns: table => new
                {
                    AlbumsId = table.Column<int>(type: "integer", nullable: false),
                    ArtistsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumArtist", x => new { x.AlbumsId, x.ArtistsId });
                    table.ForeignKey(
                        name: "FK_AlbumArtist_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumArtist_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumGenre",
                columns: table => new
                {
                    AlbumsId = table.Column<int>(type: "integer", nullable: false),
                    GenresId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumGenre", x => new { x.AlbumsId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_AlbumGenre_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumGenre_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistTrack",
                columns: table => new
                {
                    ArtistsId = table.Column<int>(type: "integer", nullable: false),
                    TracksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistTrack", x => new { x.ArtistsId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_ArtistTrack_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistTrack_Tracks_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreTrack",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "integer", nullable: false),
                    TracksId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreTrack", x => new { x.GenresId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_GenreTrack_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreTrack_Tracks_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtist_ArtistsId",
                table: "AlbumArtist",
                column: "ArtistsId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumGenre_GenresId",
                table: "AlbumGenre",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistTrack_TracksId",
                table: "ArtistTrack",
                column: "TracksId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreTrack_TracksId",
                table: "GenreTrack",
                column: "TracksId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumArtist");

            migrationBuilder.DropTable(
                name: "AlbumGenre");

            migrationBuilder.DropTable(
                name: "ArtistTrack");

            migrationBuilder.DropTable(
                name: "GenreTrack");

            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "Genres",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Genres",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "Artists",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Artists",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_AlbumId",
                table: "Genres",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_TrackId",
                table: "Genres",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_AlbumId",
                table: "Artists",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_TrackId",
                table: "Artists",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Albums_AlbumId",
                table: "Artists",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Tracks_TrackId",
                table: "Artists",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Albums_AlbumId",
                table: "Genres",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Tracks_TrackId",
                table: "Genres",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
