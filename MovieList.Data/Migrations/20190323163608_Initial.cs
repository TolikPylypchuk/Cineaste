using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieList.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kinds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    ColorForMovie = table.Column<string>(nullable: true),
                    ColorForSeries = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieSeries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsLooselyConnected = table.Column<bool>(nullable: false),
                    ParentSeriesId = table.Column<int>(nullable: true),
                    OrdinalNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieSeries_MovieSeries_ParentSeriesId",
                        column: x => x.ParentSeriesId,
                        principalTable: "MovieSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(nullable: false),
                    IsWatched = table.Column<bool>(nullable: false),
                    IsReleased = table.Column<bool>(nullable: false),
                    ImdbLink = table.Column<string>(maxLength: 256, nullable: true),
                    PosterUrl = table.Column<string>(maxLength: 256, nullable: true),
                    KindId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Kinds_KindId",
                        column: x => x.KindId,
                        principalTable: "Kinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsWatched = table.Column<bool>(nullable: false),
                    ImdbLink = table.Column<string>(maxLength: 256, nullable: true),
                    PosterUrl = table.Column<string>(maxLength: 256, nullable: true),
                    KindId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_Kinds_KindId",
                        column: x => x.KindId,
                        principalTable: "Kinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieSeriesEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<int>(nullable: true),
                    SeriesId = table.Column<int>(nullable: true),
                    MovieSeriesId = table.Column<int>(nullable: false),
                    OrdinalNumber = table.Column<int>(nullable: false),
                    DisplayNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSeriesEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieSeriesEntries_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieSeriesEntries_Series_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieSeriesEntries_MovieSeries_MovieSeriesId",
                        column: x => x.MovieSeriesId,
                        principalTable: "MovieSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsWatched = table.Column<bool>(nullable: false),
                    IsReleased = table.Column<bool>(nullable: false),
                    Channel = table.Column<string>(maxLength: 64, nullable: false),
                    ImdbLink = table.Column<string>(maxLength: 256, nullable: true),
                    PosterUrl = table.Column<string>(maxLength: 256, nullable: true),
                    SeriesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seasons_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialEpisodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    IsWatched = table.Column<bool>(nullable: false),
                    IsReleased = table.Column<bool>(nullable: false),
                    Channel = table.Column<string>(maxLength: 64, nullable: false),
                    ImdbLink = table.Column<string>(maxLength: 256, nullable: true),
                    PosterUrl = table.Column<string>(maxLength: 256, nullable: true),
                    SeriesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialEpisodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialEpisodes_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartMonth = table.Column<int>(nullable: false),
                    StartYear = table.Column<int>(nullable: false),
                    EndMonth = table.Column<int>(nullable: false),
                    EndYear = table.Column<int>(nullable: false),
                    IsSingleDayRelease = table.Column<bool>(nullable: false),
                    NumberOfEpisodes = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Periods_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Titles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    IsOriginal = table.Column<bool>(nullable: false),
                    MovieId = table.Column<int>(nullable: true),
                    SeriesId = table.Column<int>(nullable: true),
                    MovieSeriesId = table.Column<int>(nullable: true),
                    SeasonId = table.Column<int>(nullable: true),
                    SpecialEpisodeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Titles_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Titles_MovieSeries_MovieSeriesId",
                        column: x => x.MovieSeriesId,
                        principalTable: "MovieSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Titles_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Titles_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Titles_SpecialEpisodes_SpecialEpisodeId",
                        column: x => x.SpecialEpisodeId,
                        principalTable: "SpecialEpisodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_KindId",
                table: "Movies",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSeries_ParentSeriesId",
                table: "MovieSeries",
                column: "ParentSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSeriesEntries_MovieId",
                table: "MovieSeriesEntries",
                column: "MovieId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieSeriesEntries_MovieSeriesId",
                table: "MovieSeriesEntries",
                column: "MovieSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_SeasonId",
                table: "Periods",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeriesId",
                table: "Seasons",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_KindId",
                table: "Series",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialEpisodes_SeriesId",
                table: "SpecialEpisodes",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_MovieId",
                table: "Titles",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_MovieSeriesId",
                table: "Titles",
                column: "MovieSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_SeasonId",
                table: "Titles",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_SeriesId",
                table: "Titles",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_SpecialEpisodeId",
                table: "Titles",
                column: "SpecialEpisodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieSeriesEntries");

            migrationBuilder.DropTable(
                name: "Periods");

            migrationBuilder.DropTable(
                name: "Titles");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "MovieSeries");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "SpecialEpisodes");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Kinds");
        }
    }
}
