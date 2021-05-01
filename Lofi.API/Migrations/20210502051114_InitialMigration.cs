using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Lofi.API.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    desription = table.Column<string>(type: "text", nullable: true),
                    parent_genre_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genres", x => x.id);
                    table.ForeignKey(
                        name: "fk_genres_genres_parent_genre_id",
                        column: x => x.parent_genre_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "uploaded_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    unique_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    extension = table.Column<string>(type: "text", nullable: true),
                    mime_type = table.Column<string>(type: "text", nullable: false),
                    bytes = table.Column<byte[]>(type: "bytea", nullable: false),
                    hash = table.Column<string>(type: "text", nullable: false),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_uploaded_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    unique_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    cover_photo_file_id = table.Column<int>(type: "integer", nullable: false),
                    release_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums", x => x.id);
                    table.ForeignKey(
                        name: "fk_albums_uploaded_files_cover_photo_file_id",
                        column: x => x.cover_photo_file_id,
                        principalTable: "uploaded_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artists",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    wallet_address = table.Column<string>(type: "text", nullable: false),
                    profile_picture_id = table.Column<int>(type: "integer", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artists", x => x.id);
                    table.ForeignKey(
                        name: "fk_artists_uploaded_files_profile_picture_id",
                        column: x => x.profile_picture_id,
                        principalTable: "uploaded_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "album_genre",
                columns: table => new
                {
                    albums_id = table.Column<int>(type: "integer", nullable: false),
                    genres_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_genre", x => new { x.albums_id, x.genres_id });
                    table.ForeignKey(
                        name: "fk_album_genre_albums_albums_id",
                        column: x => x.albums_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_genre_genres_genres_id",
                        column: x => x.genres_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tracks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    track_number = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    cover_photo_file_id = table.Column<int>(type: "integer", nullable: true),
                    audio_file_id = table.Column<int>(type: "integer", nullable: true),
                    album_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracks", x => x.id);
                    table.ForeignKey(
                        name: "fk_tracks_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tracks_uploaded_files_audio_file_id",
                        column: x => x.audio_file_id,
                        principalTable: "uploaded_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tracks_uploaded_files_cover_photo_file_id",
                        column: x => x.cover_photo_file_id,
                        principalTable: "uploaded_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "album_artist",
                columns: table => new
                {
                    albums_id = table.Column<int>(type: "integer", nullable: false),
                    artists_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_artist", x => new { x.albums_id, x.artists_id });
                    table.ForeignKey(
                        name: "fk_album_artist_albums_albums_id",
                        column: x => x.albums_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_artist_artists_artists_id",
                        column: x => x.artists_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artist_track",
                columns: table => new
                {
                    artists_id = table.Column<int>(type: "integer", nullable: false),
                    tracks_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artist_track", x => new { x.artists_id, x.tracks_id });
                    table.ForeignKey(
                        name: "fk_artist_track_artists_artists_id",
                        column: x => x.artists_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artist_track_tracks_tracks_id",
                        column: x => x.tracks_id,
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "genre_track",
                columns: table => new
                {
                    genres_id = table.Column<int>(type: "integer", nullable: false),
                    tracks_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre_track", x => new { x.genres_id, x.tracks_id });
                    table.ForeignKey(
                        name: "fk_genre_track_genres_genres_id",
                        column: x => x.genres_id,
                        principalTable: "genres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_genre_track_tracks_tracks_id",
                        column: x => x.tracks_id,
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tips",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message = table.Column<string>(type: "text", nullable: true),
                    payment_id = table.Column<int>(type: "integer", nullable: false),
                    payment_id_hex = table.Column<string>(type: "text", nullable: false),
                    track_id = table.Column<int>(type: "integer", nullable: false),
                    integrated_payment_address = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tips", x => x.id);
                    table.ForeignKey(
                        name: "fk_tips_tracks_track_id",
                        column: x => x.track_id,
                        principalTable: "tracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artist_tip",
                columns: table => new
                {
                    artists_id = table.Column<int>(type: "integer", nullable: false),
                    tips_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artist_tip", x => new { x.artists_id, x.tips_id });
                    table.ForeignKey(
                        name: "fk_artist_tip_artists_artists_id",
                        column: x => x.artists_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artist_tip_tips_tips_id",
                        column: x => x.tips_id,
                        principalTable: "tips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tip_payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tip_id = table.Column<int>(type: "integer", nullable: false),
                    transaction_id = table.Column<string>(type: "text", nullable: false),
                    block_height = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    timestamp = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tip_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_tip_payments_tips_tip_id",
                        column: x => x.tip_id,
                        principalTable: "tips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tip_payouts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tip_id = table.Column<int>(type: "integer", nullable: false),
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    wallet_address = table.Column<string>(type: "text", nullable: true),
                    gross_payout_amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    net_payout_amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    payout_tx_fee = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    payout_tx_fee_share = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tip_payouts", x => x.id);
                    table.ForeignKey(
                        name: "fk_tip_payouts_artists_artist_id",
                        column: x => x.artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tip_payouts_tips_tip_id",
                        column: x => x.tip_id,
                        principalTable: "tips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artist_tip_payouts",
                columns: table => new
                {
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    tip_id = table.Column<int>(type: "integer", nullable: false),
                    payout_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "fk_artist_tip_payouts_artists_artist_id",
                        column: x => x.artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artist_tip_payouts_tip_payments_payout_id",
                        column: x => x.payout_id,
                        principalTable: "tip_payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artist_tip_payouts_tips_tip_id",
                        column: x => x.tip_id,
                        principalTable: "tips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_album_artist_artists_id",
                table: "album_artist",
                column: "artists_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_genre_genres_id",
                table: "album_genre",
                column: "genres_id");

            migrationBuilder.CreateIndex(
                name: "ix_albums_cover_photo_file_id",
                table: "albums",
                column: "cover_photo_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_artist_tip_tips_id",
                table: "artist_tip",
                column: "tips_id");

            migrationBuilder.CreateIndex(
                name: "ix_artist_tip_payouts_artist_id",
                table: "artist_tip_payouts",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_artist_tip_payouts_payout_id",
                table: "artist_tip_payouts",
                column: "payout_id");

            migrationBuilder.CreateIndex(
                name: "ix_artist_tip_payouts_tip_id",
                table: "artist_tip_payouts",
                column: "tip_id");

            migrationBuilder.CreateIndex(
                name: "ix_artist_track_tracks_id",
                table: "artist_track",
                column: "tracks_id");

            migrationBuilder.CreateIndex(
                name: "ix_artists_profile_picture_id",
                table: "artists",
                column: "profile_picture_id");

            migrationBuilder.CreateIndex(
                name: "ix_genre_track_tracks_id",
                table: "genre_track",
                column: "tracks_id");

            migrationBuilder.CreateIndex(
                name: "ix_genres_parent_genre_id",
                table: "genres",
                column: "parent_genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_tip_payments_tip_id",
                table: "tip_payments",
                column: "tip_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tip_payouts_artist_id",
                table: "tip_payouts",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_tip_payouts_tip_id_artist_id",
                table: "tip_payouts",
                columns: new[] { "tip_id", "artist_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tips_track_id",
                table: "tips",
                column: "track_id");

            migrationBuilder.CreateIndex(
                name: "ix_tracks_album_id",
                table: "tracks",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_tracks_audio_file_id",
                table: "tracks",
                column: "audio_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_tracks_cover_photo_file_id",
                table: "tracks",
                column: "cover_photo_file_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "album_artist");

            migrationBuilder.DropTable(
                name: "album_genre");

            migrationBuilder.DropTable(
                name: "artist_tip");

            migrationBuilder.DropTable(
                name: "artist_tip_payouts");

            migrationBuilder.DropTable(
                name: "artist_track");

            migrationBuilder.DropTable(
                name: "genre_track");

            migrationBuilder.DropTable(
                name: "tip_payouts");

            migrationBuilder.DropTable(
                name: "tip_payments");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "artists");

            migrationBuilder.DropTable(
                name: "tips");

            migrationBuilder.DropTable(
                name: "tracks");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "uploaded_files");
        }
    }
}
