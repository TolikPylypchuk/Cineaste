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
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsWatched = table.Column<bool>(type: "bit", nullable: false),
                    IsReleased = table.Column<bool>(type: "bit", nullable: false),
                    ImdbId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RottenTomatoesLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Poster = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.CheckConstraint("CH_Movies_YearPositive", "Year > 0");
                });

            migrationBuilder.CreateTable(
                name: "MovieTitles",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieTitles", x => new { x.MovieId, x.Id });
                    table.CheckConstraint("CH_MovieTitles_NameNonEmpty", "Name <> ''");
                    table.CheckConstraint("CH_MovieTitles_PriorityPositive", "Priority > 0");
                    table.ForeignKey(
                        name: "FK_MovieTitles_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieTitles");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
