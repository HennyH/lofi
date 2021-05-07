﻿// <auto-generated />
using System;
using Lofi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Lofi.Database.Migrations
{
    [DbContext(typeof(LofiContext))]
    partial class LofiContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("AlbumArtist", b =>
                {
                    b.Property<int>("AlbumsId")
                        .HasColumnType("integer")
                        .HasColumnName("albums_id");

                    b.Property<int>("ArtistsId")
                        .HasColumnType("integer")
                        .HasColumnName("artists_id");

                    b.HasKey("AlbumsId", "ArtistsId")
                        .HasName("pk_album_artist");

                    b.HasIndex("ArtistsId")
                        .HasDatabaseName("ix_album_artist_artists_id");

                    b.ToTable("album_artist");
                });

            modelBuilder.Entity("AlbumGenre", b =>
                {
                    b.Property<int>("AlbumsId")
                        .HasColumnType("integer")
                        .HasColumnName("albums_id");

                    b.Property<int>("GenresId")
                        .HasColumnType("integer")
                        .HasColumnName("genres_id");

                    b.HasKey("AlbumsId", "GenresId")
                        .HasName("pk_album_genre");

                    b.HasIndex("GenresId")
                        .HasDatabaseName("ix_album_genre_genres_id");

                    b.ToTable("album_genre");
                });

            modelBuilder.Entity("ArtistTip", b =>
                {
                    b.Property<int>("ArtistsId")
                        .HasColumnType("integer")
                        .HasColumnName("artists_id");

                    b.Property<int>("TipsId")
                        .HasColumnType("integer")
                        .HasColumnName("tips_id");

                    b.HasKey("ArtistsId", "TipsId")
                        .HasName("pk_artist_tip");

                    b.HasIndex("TipsId")
                        .HasDatabaseName("ix_artist_tip_tips_id");

                    b.ToTable("artist_tip");
                });

            modelBuilder.Entity("ArtistTrack", b =>
                {
                    b.Property<int>("ArtistsId")
                        .HasColumnType("integer")
                        .HasColumnName("artists_id");

                    b.Property<int>("TracksId")
                        .HasColumnType("integer")
                        .HasColumnName("tracks_id");

                    b.HasKey("ArtistsId", "TracksId")
                        .HasName("pk_artist_track");

                    b.HasIndex("TracksId")
                        .HasDatabaseName("ix_artist_track_tracks_id");

                    b.ToTable("artist_track");
                });

            modelBuilder.Entity("GenreTrack", b =>
                {
                    b.Property<int>("GenresId")
                        .HasColumnType("integer")
                        .HasColumnName("genres_id");

                    b.Property<int>("TracksId")
                        .HasColumnType("integer")
                        .HasColumnName("tracks_id");

                    b.HasKey("GenresId", "TracksId")
                        .HasName("pk_genre_track");

                    b.HasIndex("TracksId")
                        .HasDatabaseName("ix_genre_track_tracks_id");

                    b.ToTable("genre_track");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CoverPhotoFileId")
                        .HasColumnType("integer")
                        .HasColumnName("cover_photo_file_id");

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTime?>("ModifiedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_date");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("release_date");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<Guid?>("UniqueId")
                        .IsRequired()
                        .HasColumnType("uuid")
                        .HasColumnName("unique_id");

                    b.HasKey("Id")
                        .HasName("pk_albums");

                    b.HasIndex("CoverPhotoFileId")
                        .HasDatabaseName("ix_albums_cover_photo_file_id");

                    b.ToTable("albums");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("ProfilePictureId")
                        .HasColumnType("integer")
                        .HasColumnName("profile_picture_id");

                    b.Property<string>("WalletAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("wallet_address");

                    b.HasKey("Id")
                        .HasName("pk_artists");

                    b.HasIndex("ProfilePictureId")
                        .HasDatabaseName("ix_artists_profile_picture_id");

                    b.ToTable("artists");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.ArtistTipPayout", b =>
                {
                    b.Property<int>("ArtistId")
                        .HasColumnType("integer")
                        .HasColumnName("artist_id");

                    b.Property<int>("PayoutId")
                        .HasColumnType("integer")
                        .HasColumnName("payout_id");

                    b.Property<int>("TipId")
                        .HasColumnType("integer")
                        .HasColumnName("tip_id");

                    b.HasIndex("ArtistId")
                        .HasDatabaseName("ix_artist_tip_payouts_artist_id");

                    b.HasIndex("PayoutId")
                        .HasDatabaseName("ix_artist_tip_payouts_payout_id");

                    b.HasIndex("TipId")
                        .HasDatabaseName("ix_artist_tip_payouts_tip_id");

                    b.ToTable("artist_tip_payouts");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Desription")
                        .HasColumnType("text")
                        .HasColumnName("desription");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("ParentGenreId")
                        .HasColumnType("integer")
                        .HasColumnName("parent_genre_id");

                    b.HasKey("Id")
                        .HasName("pk_genres");

                    b.HasIndex("ParentGenreId")
                        .HasDatabaseName("ix_genres_parent_genre_id");

                    b.ToTable("genres");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Tip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("IntegratedPaymentAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("integrated_payment_address");

                    b.Property<string>("Message")
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<DateTime?>("ModifiedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_date");

                    b.Property<int>("PaymentId")
                        .HasColumnType("integer")
                        .HasColumnName("payment_id");

                    b.Property<string>("PaymentIdHex")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("payment_id_hex");

                    b.Property<int?>("TrackId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("track_id");

                    b.HasKey("Id")
                        .HasName("pk_tips");

                    b.HasIndex("TrackId")
                        .HasDatabaseName("ix_tips_track_id");

                    b.ToTable("tips");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.TipPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_date");

                    b.Property<int?>("PaymentTransferId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("payment_transfer_id");

                    b.Property<int?>("TipId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("tip_id");

                    b.HasKey("Id")
                        .HasName("pk_tip_payments");

                    b.HasIndex("PaymentTransferId")
                        .HasDatabaseName("ix_tip_payments_payment_transfer_id");

                    b.HasIndex("TipId")
                        .HasDatabaseName("ix_tip_payments_tip_id");

                    b.ToTable("tip_payments");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.TipPayout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("amount");

                    b.Property<int?>("ArtistId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("artist_id");

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("ModifiedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_date");

                    b.Property<int?>("ReceiptId")
                        .HasColumnType("integer")
                        .HasColumnName("receipt_id");

                    b.Property<int?>("TipPaymentId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("tip_payment_id");

                    b.HasKey("Id")
                        .HasName("pk_tip_payouts");

                    b.HasIndex("ArtistId")
                        .HasDatabaseName("ix_tip_payouts_artist_id");

                    b.HasIndex("ReceiptId")
                        .HasDatabaseName("ix_tip_payouts_receipt_id");

                    b.HasIndex(new[] { "TipPaymentId", "ArtistId" }, "UIX_TipPayount_TipPayment_Artist")
                        .IsUnique()
                        .HasDatabaseName("ix_tip_payouts_tip_payment_id_artist_id");

                    b.ToTable("tip_payouts");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AlbumId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("album_id");

                    b.Property<int?>("AudioFileId")
                        .HasColumnType("integer")
                        .HasColumnName("audio_file_id");

                    b.Property<int?>("CoverPhotoFileId")
                        .HasColumnType("integer")
                        .HasColumnName("cover_photo_file_id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<int?>("TrackNumber")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("track_number");

                    b.HasKey("Id")
                        .HasName("pk_tracks");

                    b.HasIndex("AlbumId")
                        .HasDatabaseName("ix_tracks_album_id");

                    b.HasIndex("AudioFileId")
                        .HasDatabaseName("ix_tracks_audio_file_id");

                    b.HasIndex("CoverPhotoFileId")
                        .HasDatabaseName("ix_tracks_cover_photo_file_id");

                    b.ToTable("tracks");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.UploadedFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<byte[]>("Bytes")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("bytes");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean")
                        .HasColumnName("deleted");

                    b.Property<string>("Extension")
                        .HasColumnType("text")
                        .HasColumnName("extension");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_name");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("hash");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("mime_type");

                    b.Property<Guid?>("UniqueId")
                        .IsRequired()
                        .HasColumnType("uuid")
                        .HasColumnName("unique_id");

                    b.HasKey("Id")
                        .HasName("pk_uploaded_files");

                    b.ToTable("uploaded_files");
                });

            modelBuilder.Entity("Lofi.Database.Entities.TipPayoutReceipt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<DateTime?>("ModifiedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_date");

                    b.Property<decimal>("NetPayoutAmount")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("net_payout_amount");

                    b.Property<decimal>("PayoutTxFee")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("payout_tx_fee");

                    b.Property<decimal>("PayoutTxFeeShare")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("payout_tx_fee_share");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("transaction_id");

                    b.HasKey("Id")
                        .HasName("pk_tip_payout_receipts");

                    b.ToTable("tip_payout_receipts");
                });

            modelBuilder.Entity("Lofi.Database.Entities.Transfer", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("amount");

                    b.Property<decimal?>("BlockHeight")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("block_height");

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("FromWalletAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("from_wallet_address");

                    b.Property<DateTime?>("ModifiedDate")
                        .IsRequired()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified_date");

                    b.Property<int>("PaymentId")
                        .HasColumnType("integer")
                        .HasColumnName("payment_id");

                    b.Property<decimal>("Timestamp")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("timestamp");

                    b.Property<string>("ToWalletAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("to_wallet_address");

                    b.Property<decimal>("TransactionFee")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("transaction_fee");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("transaction_id");

                    b.HasKey("Id")
                        .HasName("pk_transfers");

                    b.ToTable("transfers");
                });

            modelBuilder.Entity("AlbumArtist", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Album", null)
                        .WithMany()
                        .HasForeignKey("AlbumsId")
                        .HasConstraintName("fk_album_artist_albums_albums_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.Artist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .HasConstraintName("fk_album_artist_artists_artists_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AlbumGenre", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Album", null)
                        .WithMany()
                        .HasForeignKey("AlbumsId")
                        .HasConstraintName("fk_album_genre_albums_albums_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .HasConstraintName("fk_album_genre_genres_genres_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ArtistTip", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Artist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .HasConstraintName("fk_artist_tip_artists_artists_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.Tip", null)
                        .WithMany()
                        .HasForeignKey("TipsId")
                        .HasConstraintName("fk_artist_tip_tips_tips_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ArtistTrack", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Artist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .HasConstraintName("fk_artist_track_artists_artists_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.Track", null)
                        .WithMany()
                        .HasForeignKey("TracksId")
                        .HasConstraintName("fk_artist_track_tracks_tracks_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GenreTrack", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .HasConstraintName("fk_genre_track_genres_genres_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.Track", null)
                        .WithMany()
                        .HasForeignKey("TracksId")
                        .HasConstraintName("fk_genre_track_tracks_tracks_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Album", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.UploadedFile", "CoverPhotoFile")
                        .WithMany()
                        .HasForeignKey("CoverPhotoFileId")
                        .HasConstraintName("fk_albums_uploaded_files_cover_photo_file_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CoverPhotoFile");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Artist", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.UploadedFile", "ProfilePicture")
                        .WithMany()
                        .HasForeignKey("ProfilePictureId")
                        .HasConstraintName("fk_artists_uploaded_files_profile_picture_id");

                    b.Navigation("ProfilePicture");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.ArtistTipPayout", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId")
                        .HasConstraintName("fk_artist_tip_payouts_artists_artist_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.TipPayout", "Payout")
                        .WithMany()
                        .HasForeignKey("PayoutId")
                        .HasConstraintName("fk_artist_tip_payouts_tip_payouts_payout_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.Tip", "Tip")
                        .WithMany()
                        .HasForeignKey("TipId")
                        .HasConstraintName("fk_artist_tip_payouts_tips_tip_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Payout");

                    b.Navigation("Tip");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Genre", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Genre", "ParentGenre")
                        .WithMany()
                        .HasForeignKey("ParentGenreId")
                        .HasConstraintName("fk_genres_genres_parent_genre_id");

                    b.Navigation("ParentGenre");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Tip", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Track", "Track")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .HasConstraintName("fk_tips_tracks_track_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Track");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.TipPayment", b =>
                {
                    b.HasOne("Lofi.Database.Entities.Transfer", "PaymentTransfer")
                        .WithMany()
                        .HasForeignKey("PaymentTransferId")
                        .HasConstraintName("fk_tip_payments_transfers_payment_transfer_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.Tip", "Tip")
                        .WithMany("Payments")
                        .HasForeignKey("TipId")
                        .HasConstraintName("fk_tip_payments_tips_tip_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PaymentTransfer");

                    b.Navigation("Tip");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.TipPayout", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId")
                        .HasConstraintName("fk_tip_payouts_artists_artist_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.Database.Entities.TipPayoutReceipt", "Receipt")
                        .WithMany("TipPayouts")
                        .HasForeignKey("ReceiptId")
                        .HasConstraintName("fk_tip_payouts_tip_payout_receipts_receipt_id");

                    b.HasOne("Lofi.API.Database.Entities.TipPayment", "TipPayment")
                        .WithMany("Payouts")
                        .HasForeignKey("TipPaymentId")
                        .HasConstraintName("fk_tip_payouts_tip_payments_tip_payment_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Receipt");

                    b.Navigation("TipPayment");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Track", b =>
                {
                    b.HasOne("Lofi.API.Database.Entities.Album", "Album")
                        .WithMany("Tracks")
                        .HasForeignKey("AlbumId")
                        .HasConstraintName("fk_tracks_albums_album_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Lofi.API.Database.Entities.UploadedFile", "AudioFile")
                        .WithMany()
                        .HasForeignKey("AudioFileId")
                        .HasConstraintName("fk_tracks_uploaded_files_audio_file_id");

                    b.HasOne("Lofi.API.Database.Entities.UploadedFile", "CoverPhotoFile")
                        .WithMany()
                        .HasForeignKey("CoverPhotoFileId")
                        .HasConstraintName("fk_tracks_uploaded_files_cover_photo_file_id");

                    b.Navigation("Album");

                    b.Navigation("AudioFile");

                    b.Navigation("CoverPhotoFile");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Album", b =>
                {
                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.Tip", b =>
                {
                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Lofi.API.Database.Entities.TipPayment", b =>
                {
                    b.Navigation("Payouts");
                });

            modelBuilder.Entity("Lofi.Database.Entities.TipPayoutReceipt", b =>
                {
                    b.Navigation("TipPayouts");
                });
#pragma warning restore 612, 618
        }
    }
}
