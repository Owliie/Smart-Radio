using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRadio.Data.Migrations
{
    public partial class SongDataOffsetType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Offset",
                table: "Fingerprints",
                nullable: false,
                oldClrType: typeof(short));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Offset",
                table: "Fingerprints",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
