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
                name: "ListConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSeasonTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSeasonOriginalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultFirstSortOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultFirstSortDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSecondSortOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSecondSortDirection = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieKinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotWatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotReleasedColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieKinds", x => x.Id);
                    table.CheckConstraint("CH_MovieKinds_NameNotEmpty", "Name <> ''");
                });

            migrationBuilder.CreateTable(
                name: "SeriesKinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotWatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotReleasedColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesKinds", x => x.Id);
                    table.CheckConstraint("CH_SeriesKinds_NameNotEmpty", "Name <> ''");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApplicableToMovies = table.Column<bool>(type: "bit", nullable: false),
                    IsApplicableToSeries = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.CheckConstraint("CH_Tag_CategoryNotEmpty", "Category <> ''");
                    table.CheckConstraint("CH_Tag_NameNotEmpty", "Name <> ''");
                });

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
                    Poster = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    KindId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.CheckConstraint("CH_Movies_YearPositive", "Year > 0");
                    table.ForeignKey(
                        name: "FK_Movies_MovieKinds_KindId",
                        column: x => x.KindId,
                        principalTable: "MovieKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagImplications",
                columns: table => new
                {
                    ImpliedTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImplyingTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagImplications", x => new { x.ImpliedTagId, x.ImplyingTagId });
                    table.ForeignKey(
                        name: "FK_TagImplications_Tags_ImpliedTagId",
                        column: x => x.ImpliedTagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagImplications_Tags_ImplyingTagId",
                        column: x => x.ImplyingTagId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MovieTags",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieTags", x => new { x.MovieId, x.Id });
                    table.ForeignKey(
                        name: "FK_MovieTags_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.CheckConstraint("CH_MovieTitles_NameNotEmpty", "Name <> ''");
                    table.CheckConstraint("CH_MovieTitles_PriorityPositive", "Priority > 0");
                    table.ForeignKey(
                        name: "FK_MovieTitles_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieKinds_Name",
                table: "MovieKinds",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_KindId",
                table: "Movies",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieTags_TagId",
                table: "MovieTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesKinds_Name",
                table: "SeriesKinds",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagImplications_ImplyingTagId",
                table: "TagImplications",
                column: "ImplyingTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name_Category",
                table: "Tags",
                columns: new[] { "Name", "Category" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListConfiguration");

            migrationBuilder.DropTable(
                name: "MovieTags");

            migrationBuilder.DropTable(
                name: "MovieTitles");

            migrationBuilder.DropTable(
                name: "SeriesKinds");

            migrationBuilder.DropTable(
                name: "TagImplications");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "MovieKinds");
        }
    }
}
