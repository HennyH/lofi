using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Lofi.API.Migrations
{
    public partial class SplitUpTip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "BlockHeight",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "PayoutAmount",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "PayoutDate",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "PayoutTransactionId",
                table: "Tips");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Tips");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Tips",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ArtistTip",
                columns: table => new
                {
                    ArtistsId = table.Column<int>(type: "integer", nullable: false),
                    TipsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistTip", x => new { x.ArtistsId, x.TipsId });
                    table.ForeignKey(
                        name: "FK_ArtistTip_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistTip_Tips_TipsId",
                        column: x => x.TipsId,
                        principalTable: "Tips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipId = table.Column<int>(type: "integer", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: false),
                    BlockHeight = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Timestamp = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipPayment_Tips_TipId",
                        column: x => x.TipId,
                        principalTable: "Tips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipPayout",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipId = table.Column<int>(type: "integer", nullable: false),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    WalletAddress = table.Column<string>(type: "text", nullable: true),
                    TipAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GrossPayoutAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    NetPayoutAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PayoutTxFee = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PayoutTxFeeShare = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipPayout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipPayout_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TipPayout_Tips_TipId",
                        column: x => x.TipId,
                        principalTable: "Tips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistTip_TipsId",
                table: "ArtistTip",
                column: "TipsId");

            migrationBuilder.CreateIndex(
                name: "IX_TipPayment_TipId",
                table: "TipPayment",
                column: "TipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipPayout_ArtistId",
                table: "TipPayout",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "UIX_TipPayount_Tip_Artist",
                table: "TipPayout",
                columns: new[] { "TipId", "ArtistId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistTip");

            migrationBuilder.DropTable(
                name: "TipPayment");

            migrationBuilder.DropTable(
                name: "TipPayout");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Tips");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Tips",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BlockHeight",
                table: "Tips",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PayoutAmount",
                table: "Tips",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PayoutDate",
                table: "Tips",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayoutTransactionId",
                table: "Tips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Tips",
                type: "text",
                nullable: true);
        }
    }
}
