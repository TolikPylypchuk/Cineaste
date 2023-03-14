namespace Cineaste.Server.Services;

using System.Globalization;

using Cineaste.Basic;
using Cineaste.Core.Domain;
using Cineaste.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Testcontainers.MsSql;

public abstract class ServiceTestsBase : IAsyncLifetime
{
    private readonly MsSqlContainer container = new MsSqlBuilder().Build();

    protected ILogger Logger { get; }

    protected CineasteList List { get; }
    protected MovieKind MovieKind { get; }
    protected SeriesKind SeriesKind { get; }

    protected ServiceTestsBase(ITestOutputHelper output)
    {
        this.Logger = XUnitLogger.CreateLogger(this.GetType(), output);

        this.List = this.CreateList($"List.{this.GetType().Name}", $"list.{this.GetType().Name.ToLower()}");
        this.MovieKind = this.CreateMovieKind(this.List);
        this.SeriesKind = this.CreateSeriesKind(this.List);
    }

    public async Task InitializeAsync() =>
        await this.container.StartAsync();

    public async Task DisposeAsync() =>
        await this.container.DisposeAsync().AsTask();

    protected CineasteList CreateList(string name, string handle) =>
        new(
            Id.CreateNew<CineasteList>(),
            name,
            handle,
            new ListConfiguration(
                Id.CreateNew<ListConfiguration>(),
                CultureInfo.InvariantCulture,
                "Season #",
                "Season #",
                ListSortingConfiguration.CreateDefault()));

    protected Movie CreateMovie(CineasteList list)
    {
        var movie = new Movie(Id.CreateNew<Movie>(), this.CreateTitles(), 2000, true, true, this.MovieKind);
        list.AddMovie(movie);

        return movie;
    }

    protected Series CreateSeries(CineasteList list)
    {
        var series = new Series(
            Id.CreateNew<Series>(),
            this.CreateTitles(),
            new List<Season> { this.CreateSeason(1), this.CreateSeason(2) },
            new List<SpecialEpisode> { this.CreateSpecialEpisode(3) },
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            this.SeriesKind);

        list.AddSeries(series);

        return series;
    }

    protected Season CreateSeason(int num) =>
        new(
            Id.CreateNew<Season>(),
            new List<Title> { new($"Season {num}", 1, false), new($"Season {num}", 1, true) },
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.Finished,
            "Test",
            num,
            new List<Period>
            {
                new(Id.CreateNew<Period>(), 1, 2000 + num, 2, 2000 + num, false, 10)
            });

    protected SpecialEpisode CreateSpecialEpisode(int num) =>
        new(Id.CreateNew<SpecialEpisode>(), this.CreateTitles(), 1, 2000 + num, false, true, "Test", num);

    protected Franchise CreateFranchise(CineasteList list)
    {
        var franchise = new Franchise(
            Id.CreateNew<Franchise>(),
            this.CreateTitles(),
            showTitles: true,
            isLooselyConnected: false,
            continueNumbering: false);

        list.AddFranchise(franchise);

        return franchise;
    }

    protected IEnumerable<Title> CreateTitles() =>
        new List<Title> { new("Test", 1, false), new("Original Test", 1, true) };

    protected MovieKind CreateMovieKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new MovieKind(Id.CreateNew<MovieKind>(), "Test Kind", black, black, black);

        list.AddMovieKind(kind);

        return kind;
    }

    protected SeriesKind CreateSeriesKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new SeriesKind(Id.CreateNew<SeriesKind>(), "Test Kind", black, black, black);

        list.AddSeriesKind(kind);

        return kind;
    }

    protected async Task<CineasteDbContext> CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CineasteDbContext>()
            .UseSqlServer(this.container.GetConnectionString())
            .Options;

        var context = new CineasteDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.MovieKinds.Add(this.MovieKind);
        context.SeriesKinds.Add(this.SeriesKind);
        context.Lists.Add(this.List);

        context.SaveChanges();

        return context;
    }
}
