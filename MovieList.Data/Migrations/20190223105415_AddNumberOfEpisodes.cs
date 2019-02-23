using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieList.Data.Migrations
{
    public partial class AddNumberOfEpisodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfEpisodes",
                table: "Periods",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfEpisodes",
                table: "Periods");
        }
    }
}
