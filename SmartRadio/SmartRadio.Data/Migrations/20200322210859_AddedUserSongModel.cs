using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRadio.Data.Migrations
{
    public partial class AddedUserSongModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_AspNetUsers_ListenerId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_ListenerId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "ListenerId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "RadioStation",
                table: "Songs");

            migrationBuilder.CreateTable(
                name: "UserSongs",
                columns: table => new
                {
                    ListenerId = table.Column<string>(nullable: false),
                    SongId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    RadioStation = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSongs", x => new { x.ListenerId, x.SongId, x.Date });
                    table.ForeignKey(
                        name: "FK_UserSongs_AspNetUsers_ListenerId",
                        column: x => x.ListenerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSongs_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSongs_SongId",
                table: "UserSongs",
                column: "SongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSongs");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Songs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ListenerId",
                table: "Songs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RadioStation",
                table: "Songs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_ListenerId",
                table: "Songs",
                column: "ListenerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_AspNetUsers_ListenerId",
                table: "Songs",
                column: "ListenerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
