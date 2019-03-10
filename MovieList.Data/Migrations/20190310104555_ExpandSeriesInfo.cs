using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieList.Data.Migrations
{
    public partial class ExpandSeriesInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "SpecialEpisodes",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Seasons",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsSingleDayRelease",
                table: "Periods",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "SpecialEpisodes");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "IsSingleDayRelease",
                table: "Periods");
        }
    }
}
