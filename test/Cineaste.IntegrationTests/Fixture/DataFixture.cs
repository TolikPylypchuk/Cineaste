using System.Globalization;

using Microsoft.EntityFrameworkCore;

using Testcontainers.MsSql;

namespace Cineaste.Fixture;

public class DataFixture : IAsyncLifetime
{
    private readonly MsSqlContainer container = new MsSqlBuilder()
        .WithReuse(true)
        .WithLabel("reuse-id", "DB")
        .Build();

    private readonly CineasteList list;
    private readonly MovieKind movieKind;
    private readonly SeriesKind seriesKind;

    public DataFixture()
    {
        this.list = this.CreateList();
        this.movieKind = this.CreateMovieKind(this.list);
        this.seriesKind = this.CreateSeriesKind(this.list);
    }

    public Id<CineasteList> ListId => this.list.Id;
    public Id<MovieKind>  MovieKindId => this.movieKind.Id;
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
        dbContext.Lists.Where(list => list.Id ==  this.list.Id).SingleAsync();

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

    public Franchise CreateFranchise(CineasteList list)
    {
        var franchise = new Franchise(
            Id.Create<Franchise>(),
            this.CreateTitles(),
            showTitles: true,
            isLooselyConnected: false,
            continueNumbering: false);

        list.AddFranchise(franchise);

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
}
