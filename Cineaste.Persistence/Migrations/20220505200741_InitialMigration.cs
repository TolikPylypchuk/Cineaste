using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cineaste.Persistence.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsWatched = table.Column<bool>(type: "bit", nullable: false),
                    IsReleased = table.Column<bool>(type: "bit", nullable: false),
                    ImdbId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RottenTomatoesLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                    table.CheckConstraint("CH_Movie_YearPositive", "Year > 0");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movie");
        }
    }
}
