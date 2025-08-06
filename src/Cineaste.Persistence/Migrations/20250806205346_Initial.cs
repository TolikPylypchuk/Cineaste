using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cineaste.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSeasonTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSeasonOriginalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstSortOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondSortOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListConfigurations_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieKinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotWatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotReleasedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieKinds", x => x.Id);
                    table.CheckConstraint("CH_MovieKinds_NameNotEmpty", "Name <> ''");
                    table.ForeignKey(
                        name: "FK_MovieKinds_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesKinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotWatchedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotReleasedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesKinds", x => x.Id);
                    table.CheckConstraint("CH_SeriesKinds_NameNotEmpty", "Name <> ''");
                    table.ForeignKey(
                        name: "FK_SeriesKinds_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    IsApplicableToSeries = table.Column<bool>(type: "bit", nullable: false),
                    ListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.CheckConstraint("CH_Tag_CategoryNotEmpty", "Category <> ''");
                    table.CheckConstraint("CH_Tag_NameNotEmpty", "Name <> ''");
                    table.ForeignKey(
                        name: "FK_Tags_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id");
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
                name: "FranchiseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentFranchiseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    DisplayNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FranchiseItems", x => x.Id);
                    table.CheckConstraint("CH_FranchiseItems_SequenceNumberPositive", "SequenceNumber > 0");
                });

            migrationBuilder.CreateTable(
                name: "Franchises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowTitles = table.Column<bool>(type: "bit", nullable: false),
                    IsLooselyConnected = table.Column<bool>(type: "bit", nullable: false),
                    ContinueNumbering = table.Column<bool>(type: "bit", nullable: false),
                    MovieKindId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeriesKindId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KindSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosterHash = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: true),
                    FranchiseItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Franchises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Franchises_FranchiseItems_FranchiseItemId",
                        column: x => x.FranchiseItemId,
                        principalTable: "FranchiseItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Franchises_MovieKinds_MovieKindId",
                        column: x => x.MovieKindId,
                        principalTable: "MovieKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Franchises_SeriesKinds_SeriesKindId",
                        column: x => x.SeriesKindId,
                        principalTable: "SeriesKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsWatched = table.Column<bool>(type: "bit", nullable: false),
                    IsReleased = table.Column<bool>(type: "bit", nullable: false),
                    KindId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImdbId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RottenTomatoesId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterHash = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: true),
                    FranchiseItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.CheckConstraint("CH_Movies_YearPositive", "Year > 0");
                    table.ForeignKey(
                        name: "FK_Movies_FranchiseItems_FranchiseItemId",
                        column: x => x.FranchiseItemId,
                        principalTable: "FranchiseItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movies_MovieKinds_KindId",
                        column: x => x.KindId,
                        principalTable: "MovieKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WatchStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KindId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImdbId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RottenTomatoesId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterHash = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: true),
                    FranchiseItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_FranchiseItems_FranchiseItemId",
                        column: x => x.FranchiseItemId,
                        principalTable: "FranchiseItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Series_SeriesKinds_KindId",
                        column: x => x.KindId,
                        principalTable: "SeriesKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FranchisePosters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FranchiseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FranchisePosters", x => x.Id);
                    table.CheckConstraint("CH_FranchisePosters_ContentTypeNotEmpty", "ContentType <> ''");
                    table.CheckConstraint("CH_FranchisePosters_DataNotEmpty", "DATALENGTH(Data) > 0");
                    table.ForeignKey(
                        name: "FK_FranchisePosters_Franchises_FranchiseId",
                        column: x => x.FranchiseId,
                        principalTable: "Franchises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FranchiseTitles",
                columns: table => new
                {
                    FranchiseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FranchiseTitles", x => new { x.FranchiseId, x.Id });
                    table.CheckConstraint("CH_FranchiseTitles_NameNotEmpty", "Name <> ''");
                    table.CheckConstraint("CH_FranchiseTitles_SequenceNumberPositive", "SequenceNumber > 0");
                    table.ForeignKey(
                        name: "FK_FranchiseTitles_Franchises_FranchiseId",
                        column: x => x.FranchiseId,
                        principalTable: "Franchises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MoviePosters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviePosters", x => x.Id);
                    table.CheckConstraint("CH_MoviePosters_ContentTypeNotEmpty", "ContentType <> ''");
                    table.CheckConstraint("CH_MoviePosters_DataNotEmpty", "DATALENGTH(Data) > 0");
                    table.ForeignKey(
                        name: "FK_MoviePosters_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieTitles", x => new { x.MovieId, x.Id });
                    table.CheckConstraint("CH_MovieTitles_NameNotEmpty", "Name <> ''");
                    table.CheckConstraint("CH_MovieTitles_SequenceNumberPositive", "SequenceNumber > 0");
                    table.ForeignKey(
                        name: "FK_MovieTitles_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FranchiseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NormalizedTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedOriginalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedShortTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedShortOriginalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: true),
                    EndYear = table.Column<int>(type: "int", nullable: true),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    ActiveColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsStandalone = table.Column<bool>(type: "bit", nullable: false),
                    IsShown = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListItems", x => x.Id);
                    table.CheckConstraint("CH_ListItems_SequenceNumberNonNegative", "SequenceNumber >= 0");
                    table.ForeignKey(
                        name: "FK_ListItems_Franchises_FranchiseId",
                        column: x => x.FranchiseId,
                        principalTable: "Franchises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListItems_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListItems_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListItems_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WatchStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.CheckConstraint("CH_Seasons_ChannelNotEmpty", "Channel <> ''");
                    table.CheckConstraint("CH_Seasons_SequenceNumberPositive", "SequenceNumber > 0");
                    table.ForeignKey(
                        name: "FK_Seasons_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesPosters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesPosters", x => x.Id);
                    table.CheckConstraint("CH_SeriesPosters_ContentTypeNotEmpty", "ContentType <> ''");
                    table.CheckConstraint("CH_SeriesPosters_DataNotEmpty", "DATALENGTH(Data) > 0");
                    table.ForeignKey(
                        name: "FK_SeriesPosters_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesTags",
                columns: table => new
                {
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesTags", x => new { x.SeriesId, x.Id });
                    table.ForeignKey(
                        name: "FK_SeriesTags_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesTitles",
                columns: table => new
                {
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesTitles", x => new { x.SeriesId, x.Id });
                    table.CheckConstraint("CH_SeriesTitles_NameNotEmpty", "Name <> ''");
                    table.CheckConstraint("CH_SeriesTitles_SequenceNumberPositive", "SequenceNumber > 0");
                    table.ForeignKey(
                        name: "FK_SeriesTitles_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialEpisodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsWatched = table.Column<bool>(type: "bit", nullable: false),
                    IsReleased = table.Column<bool>(type: "bit", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    RottenTomatoesId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterHash = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: true),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialEpisodes", x => x.Id);
                    table.CheckConstraint("CH_SpecialEpisodes_ChannelNotEmpty", "Channel <> ''");
                    table.CheckConstraint("CH_SpecialEpisodes_MonthValid", "Month >= 1 AND Month <= 12");
                    table.CheckConstraint("CH_SpecialEpisodes_SequenceNumberPositive", "SequenceNumber > 0");
                    table.CheckConstraint("CH_SpecialEpisodes_YearPositive", "Year > 0");
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    EndMonth = table.Column<int>(type: "int", nullable: false),
                    EndYear = table.Column<int>(type: "int", nullable: false),
                    IsSingleDayRelease = table.Column<bool>(type: "bit", nullable: false),
                    EpisodeCount = table.Column<int>(type: "int", nullable: false),
                    RottenTomatoesId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PosterHash = table.Column<string>(type: "nchar(64)", fixedLength: true, maxLength: 64, nullable: true),
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periods", x => x.Id);
                    table.CheckConstraint("CH_Periods_EndMonthValid", "StartMonth >= 1 AND StartMonth <= 12");
                    table.CheckConstraint("CH_Periods_EndYearPositive", "EndYear > 0");
                    table.CheckConstraint("CH_Periods_EpisodeCountPositive", "EndYear > 0");
                    table.CheckConstraint("CH_Periods_PeriodValid", "DATEFROMPARTS(StartYear, StartMonth, 1) <= DATEFROMPARTS(EndYear, EndMonth, 1)");
                    table.CheckConstraint("CH_Periods_StartMonthValid", "StartMonth >= 1 AND StartMonth <= 12");
                    table.CheckConstraint("CH_Periods_StartYearPositive", "StartYear > 0");
                    table.ForeignKey(
                        name: "FK_Periods_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonTitles",
                columns: table => new
                {
                    SeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonTitles", x => new { x.SeasonId, x.Id });
                    table.CheckConstraint("CH_SeasonTitles_NameNotEmpty", "Name <> ''");
                    table.CheckConstraint("CH_SeasonTitles_SequenceNumberPositive", "SequenceNumber > 0");
                    table.ForeignKey(
                        name: "FK_SeasonTitles_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialEpisodePosters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialEpisodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialEpisodePosters", x => x.Id);
                    table.CheckConstraint("CH_SpecialEpisodePosters_ContentTypeNotEmpty", "ContentType <> ''");
                    table.CheckConstraint("CH_SpecialEpisodePosters_DataNotEmpty", "DATALENGTH(Data) > 0");
                    table.ForeignKey(
                        name: "FK_SpecialEpisodePosters_SpecialEpisodes_SpecialEpisodeId",
                        column: x => x.SpecialEpisodeId,
                        principalTable: "SpecialEpisodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialEpisodeTitles",
                columns: table => new
                {
                    SpecialEpisodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialEpisodeTitles", x => new { x.SpecialEpisodeId, x.Id });
                    table.CheckConstraint("CH_SpecialEpisodeTitles_NameNotEmpty", "Name <> ''");
                    table.CheckConstraint("CH_SpecialEpisodeTitles_SequenceNumberPositive", "SequenceNumber > 0");
                    table.ForeignKey(
                        name: "FK_SpecialEpisodeTitles_SpecialEpisodes_SpecialEpisodeId",
                        column: x => x.SpecialEpisodeId,
                        principalTable: "SpecialEpisodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonPosters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonPosters", x => x.Id);
                    table.CheckConstraint("CH_SeasonPosters_ContentTypeNotEmpty", "ContentType <> ''");
                    table.CheckConstraint("CH_SeasonPosters_DataNotEmpty", "DATALENGTH(Data) > 0");
                    table.ForeignKey(
                        name: "FK_SeasonPosters_Periods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "Periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FranchiseItems_ParentFranchiseId",
                table: "FranchiseItems",
                column: "ParentFranchiseId");

            migrationBuilder.CreateIndex(
                name: "IX_FranchisePosters_FranchiseId",
                table: "FranchisePosters",
                column: "FranchiseId");

            migrationBuilder.CreateIndex(
                name: "IX_Franchises_FranchiseItemId",
                table: "Franchises",
                column: "FranchiseItemId",
                unique: true,
                filter: "[FranchiseItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Franchises_MovieKindId",
                table: "Franchises",
                column: "MovieKindId");

            migrationBuilder.CreateIndex(
                name: "IX_Franchises_SeriesKindId",
                table: "Franchises",
                column: "SeriesKindId");

            migrationBuilder.CreateIndex(
                name: "IX_ListConfigurations_ListId",
                table: "ListConfigurations",
                column: "ListId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_FranchiseId",
                table: "ListItems",
                column: "FranchiseId",
                unique: true,
                filter: "[FranchiseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_ListId",
                table: "ListItems",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_MovieId",
                table: "ListItems",
                column: "MovieId",
                unique: true,
                filter: "[MovieId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_SequenceNumber",
                table: "ListItems",
                column: "SequenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_SeriesId",
                table: "ListItems",
                column: "SeriesId",
                unique: true,
                filter: "[SeriesId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovieKinds_ListId",
                table: "MovieKinds",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieKinds_Name_ListId",
                table: "MovieKinds",
                columns: new[] { "Name", "ListId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MoviePosters_MovieId",
                table: "MoviePosters",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_FranchiseItemId",
                table: "Movies",
                column: "FranchiseItemId",
                unique: true,
                filter: "[FranchiseItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_KindId",
                table: "Movies",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieTags_TagId",
                table: "MovieTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_SeasonId",
                table: "Periods",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPosters_PeriodId",
                table: "SeasonPosters",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeriesId",
                table: "Seasons",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_FranchiseItemId",
                table: "Series",
                column: "FranchiseItemId",
                unique: true,
                filter: "[FranchiseItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Series_KindId",
                table: "Series",
                column: "KindId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesKinds_ListId",
                table: "SeriesKinds",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesKinds_Name_ListId",
                table: "SeriesKinds",
                columns: new[] { "Name", "ListId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeriesPosters_SeriesId",
                table: "SeriesPosters",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesTags_TagId",
                table: "SeriesTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialEpisodePosters_SpecialEpisodeId",
                table: "SpecialEpisodePosters",
                column: "SpecialEpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialEpisodes_SeriesId",
                table: "SpecialEpisodes",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_TagImplications_ImplyingTagId",
                table: "TagImplications",
                column: "ImplyingTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ListId",
                table: "Tags",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name_Category",
                table: "Tags",
                columns: new[] { "Name", "Category" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FranchiseItems_Franchises_ParentFranchiseId",
                table: "FranchiseItems",
                column: "ParentFranchiseId",
                principalTable: "Franchises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FranchiseItems_Franchises_ParentFranchiseId",
                table: "FranchiseItems");

            migrationBuilder.DropTable(
                name: "FranchisePosters");

            migrationBuilder.DropTable(
                name: "FranchiseTitles");

            migrationBuilder.DropTable(
                name: "ListConfigurations");

            migrationBuilder.DropTable(
                name: "ListItems");

            migrationBuilder.DropTable(
                name: "MoviePosters");

            migrationBuilder.DropTable(
                name: "MovieTags");

            migrationBuilder.DropTable(
                name: "MovieTitles");

            migrationBuilder.DropTable(
                name: "SeasonPosters");

            migrationBuilder.DropTable(
                name: "SeasonTitles");

            migrationBuilder.DropTable(
                name: "SeriesPosters");

            migrationBuilder.DropTable(
                name: "SeriesTags");

            migrationBuilder.DropTable(
                name: "SeriesTitles");

            migrationBuilder.DropTable(
                name: "SpecialEpisodePosters");

            migrationBuilder.DropTable(
                name: "SpecialEpisodeTitles");

            migrationBuilder.DropTable(
                name: "TagImplications");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Periods");

            migrationBuilder.DropTable(
                name: "SpecialEpisodes");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Franchises");

            migrationBuilder.DropTable(
                name: "FranchiseItems");

            migrationBuilder.DropTable(
                name: "MovieKinds");

            migrationBuilder.DropTable(
                name: "SeriesKinds");

            migrationBuilder.DropTable(
                name: "Lists");
        }
    }
}
