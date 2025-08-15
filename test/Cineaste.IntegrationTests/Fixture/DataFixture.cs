using System.Globalization;

using Cineaste.Models;
using Cineaste.Services.Poster;
using Cineaste.Shared.Models.Poster;

using Microsoft.EntityFrameworkCore;

using Testcontainers.MsSql;

namespace Cineaste.Fixture;

public class DataFixture : IAsyncLifetime
{
    public static readonly byte[] PosterData = Convert.FromHexString(
        "ffd8ffe000104a46494600010101004800480000ffdb004300030202020202030202020303030304060404040404080606050609080a" +
        "0a090809090a0c0f0c0a0b0e0b09090d110d0e0f101011100a0c12131210130f101010ffc9000b080001000101011100ffcc00060010" +
        "1005ffda0008010100003f00d2cf20ffd9");

    public const string PosterType = "image/jpeg";

    public const string ImdbMediaId = "rm12345678";
    public const string ImdbImageSelector = $"img[data-image-id=\"{ImdbMediaId}-curr\"]";
    public const string Img = "img";
    public const string Src = "src";

    private readonly MsSqlContainer container = new MsSqlBuilder()
        .WithReuse(true)
        .WithLabel("reuse-id", "DB")
        .Build();

    private readonly CineasteList list;
    private readonly MovieKind movieKind;
    private readonly SeriesKind seriesKind;

    public DataFixture()
    {
        this.PosterProvider = Substitute.For<IPosterProvider>();

        this.list = this.CreateList();
        this.movieKind = this.CreateMovieKind(this.list);
        this.seriesKind = this.CreateSeriesKind(this.list);
    }

    public IPosterProvider PosterProvider { get; private set; }

    public Id<CineasteList> ListId => this.list.Id;
    public Id<MovieKind> MovieKindId => this.movieKind.Id;
    public Id<SeriesKind> SeriesKindId => this.seriesKind.Id;

    public async ValueTask InitializeAsync()
    {
        await this.container.StartAsync();

        var dbContext = this.CreateDbContext();
        await dbContext.Database.MigrateAsync(TestContext.Current.CancellationToken);

        dbContext.Tags.RemoveRange(dbContext.Tags);
        dbContext.FranchiseItems.RemoveRange(dbContext.FranchiseItems);
        dbContext.Franchises.RemoveRange(dbContext.Franchises);
        dbContext.Movies.RemoveRange(dbContext.Movies);
        dbContext.Periods.RemoveRange(dbContext.Periods);
        dbContext.Seasons.RemoveRange(dbContext.Seasons);
        dbContext.SpecialEpisodes.RemoveRange(dbContext.SpecialEpisodes);
        dbContext.Series.RemoveRange(dbContext.Series);
        dbContext.MovieKinds.RemoveRange(dbContext.MovieKinds);
        dbContext.SeriesKinds.RemoveRange(dbContext.SeriesKinds);
        dbContext.ListConfigurations.RemoveRange(dbContext.ListConfigurations);
        dbContext.ListItems.RemoveRange(dbContext.ListItems);
        dbContext.Lists.RemoveRange(dbContext.Lists);

        dbContext.MovieKinds.Add(this.movieKind);
        dbContext.SeriesKinds.Add(this.seriesKind);
        dbContext.Lists.Add(this.list);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await this.container.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public CineasteDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CineasteDbContext>()
            .UseSqlServer(this.container.GetConnectionString())
            .Options;

        return new CineasteDbContext(options);
    }

    public Task<CineasteList> GetList(CineasteDbContext dbContext) =>
        dbContext.Lists
            .Where(list => list.Id == this.list.Id)
            .Include(list => list.Configuration)
            .Include(list => list.Items)
            .Include(list => list.MovieKinds)
            .Include(list => list.SeriesKinds)
            .SingleAsync();

    public Task<MovieKind> GetMovieKind(CineasteDbContext dbContext) =>
        dbContext.MovieKinds.Where(kind => kind.Id == this.movieKind.Id).SingleAsync();

    public Task<SeriesKind> GetSeriesKind(CineasteDbContext dbContext) =>
        dbContext.SeriesKinds.Where(kind => kind.Id == this.seriesKind.Id).SingleAsync();

    public CineasteList CreateList() =>
        new(
            Id.Create<CineasteList>(),
            new ListConfiguration(
                Id.Create<ListConfiguration>(),
                CultureInfo.InvariantCulture,
                "Season #",
                "Season #",
                ListSortingConfiguration.CreateDefault()));

    public async Task<Movie> CreateMovie(CineasteDbContext dbContext)
    {
        var list = await this.GetList(dbContext);
        var kind = await this.GetMovieKind(dbContext);

        var movie = new Movie(Id.Create<Movie>(), this.CreateTitles(), 2000, true, true, kind);
        list.AddMovie(movie);
        list.SortItems();

        dbContext.Movies.Add(movie);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return movie;
    }

    public async Task<Series> CreateSeries(CineasteDbContext dbContext)
    {
        var list = await this.GetList(dbContext);
        var kind = await this.GetSeriesKind(dbContext);

        var series = new Series(
            Id.Create<Series>(),
            this.CreateTitles(),
            [this.CreateSeason(1), this.CreateSeason(2)],
            [this.CreateSpecialEpisode(3)],
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            kind);

        list.AddSeries(series);
        list.SortItems();

        dbContext.Series.Add(series);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return series;
    }

    public Season CreateSeason(int num) =>
        new(
            Id.Create<Season>(),
            [new($"Season {num}", 1, false), new($"Season {num}", 1, true)],
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.Finished,
            "Test",
            num,
            [new(Id.Create<Period>(), 1, 2000 + num, 2, 2000 + num, false, 10)]);

    public SpecialEpisode CreateSpecialEpisode(int num) =>
        new(Id.Create<SpecialEpisode>(), this.CreateTitles(), 1, 2000 + num, false, true, "Test", num);

    public async Task<Franchise> CreateFranchise(CineasteDbContext dbContext)
    {
        var list = await this.GetList(dbContext);

        var franchise = new Franchise(
            Id.Create<Franchise>(),
            this.CreateTitles(),
            await this.GetMovieKind(dbContext),
            await this.GetSeriesKind(dbContext),
            FranchiseKindSource.Movie,
            showTitles: true,
            isLooselyConnected: false,
            continueNumbering: false);

        list.AddFranchise(franchise);
        list.SortItems();

        dbContext.Franchises.Add(franchise);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return franchise;
    }

    public IEnumerable<Title> CreateTitles() =>
        [new("Test", 1, false), new("Original Test", 1, true)];

    public MovieKind CreateMovieKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new MovieKind(Id.Create<MovieKind>(), "Test Kind", black, black, black);

        list.AddMovieKind(kind);

        return kind;
    }

    public SeriesKind CreateSeriesKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new SeriesKind(Id.Create<SeriesKind>(), "Test Kind", black, black, black);

        list.AddSeriesKind(kind);

        return kind;
    }

    public async Task<MoviePoster> CreateMoviePoster(Movie movie, CineasteDbContext dbContext)
    {
        var poster = new MoviePoster(Id.Create<MoviePoster>(), movie, PosterData, PosterType);

        dbContext.MoviePosters.Add(poster);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return poster;
    }

    public async Task<SeriesPoster> CreateSeriesPoster(Series series, CineasteDbContext dbContext)
    {
        var poster = new SeriesPoster(Id.Create<SeriesPoster>(), series, PosterData, PosterType);

        dbContext.SeriesPosters.Add(poster);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return poster;
    }

    public async Task<SeasonPoster> CreateSeasonPoster(Period period, CineasteDbContext dbContext)
    {
        var poster = new SeasonPoster(Id.Create<SeasonPoster>(), period, PosterData, PosterType);

        dbContext.SeasonPosters.Add(poster);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return poster;
    }

    public async Task<SpecialEpisodePoster> CreateSpecialEpisodePoster(
        SpecialEpisode episode,
        CineasteDbContext dbContext)
    {
        var poster = new SpecialEpisodePoster(Id.Create<SpecialEpisodePoster>(), episode, PosterData, PosterType);

        dbContext.SpecialEpisodePosters.Add(poster);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return poster;
    }

    public async Task<FranchisePoster> CreateFranchisePoster(Franchise franchise, CineasteDbContext dbContext)
    {
        var poster = new FranchisePoster(Id.Create<FranchisePoster>(), franchise, PosterData, PosterType);

        dbContext.FranchisePosters.Add(poster);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        return poster;
    }

    public StreamableContent CreatePosterContent() =>
        new(() => new MemoryStream(PosterData), PosterData.Length, PosterType);

    public Validated<PosterUrlRequest> CreatePosterUrlRequest() =>
        new PosterUrlRequest("https://tolik.io").Validated();

    public Validated<PosterImdbMediaRequest> CreatePosterImdbMediaRequest() =>
        new PosterImdbMediaRequest($"https://tolik.io/{ImdbMediaId}").Validated();
}
