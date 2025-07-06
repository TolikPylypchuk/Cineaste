using System.Globalization;

using Cineaste.Persistence;

using Microsoft.EntityFrameworkCore;

using Testcontainers.MsSql;

namespace Cineaste.Server.Services;

public abstract class ServiceTestsBase : IAsyncLifetime
{
    private readonly MsSqlContainer container = new MsSqlBuilder().Build();

    protected CineasteList List { get; }
    protected MovieKind MovieKind { get; }
    protected SeriesKind SeriesKind { get; }

    protected ServiceTestsBase()
    {
        this.List = this.CreateList();
        this.MovieKind = this.CreateMovieKind(this.List);
        this.SeriesKind = this.CreateSeriesKind(this.List);
    }

    public async ValueTask InitializeAsync() =>
        await this.container.StartAsync();

    public async ValueTask DisposeAsync()
    {
        await this.container.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected CineasteList CreateList() =>
        new(
            Id.Create<CineasteList>(),
            new ListConfiguration(
                Id.Create<ListConfiguration>(),
                CultureInfo.InvariantCulture,
                "Season #",
                "Season #",
                ListSortingConfiguration.CreateDefault()));

    protected Movie CreateMovie(CineasteList list)
    {
        var movie = new Movie(Id.Create<Movie>(), this.CreateTitles(), 2000, true, true, this.MovieKind);
        list.AddMovie(movie);

        return movie;
    }

    protected Series CreateSeries(CineasteList list)
    {
        var series = new Series(
            Id.Create<Series>(),
            this.CreateTitles(),
            [this.CreateSeason(1), this.CreateSeason(2)],
            [this.CreateSpecialEpisode(3)],
            SeriesWatchStatus.NotWatched,
            SeriesReleaseStatus.Finished,
            this.SeriesKind);

        list.AddSeries(series);

        return series;
    }

    protected Season CreateSeason(int num) =>
        new(
            Id.Create<Season>(),
            [new($"Season {num}", 1, false), new($"Season {num}", 1, true)],
            SeasonWatchStatus.NotWatched,
            SeasonReleaseStatus.Finished,
            "Test",
            num,
            [new(Id.Create<Period>(), 1, 2000 + num, 2, 2000 + num, false, 10)]);

    protected SpecialEpisode CreateSpecialEpisode(int num) =>
        new(Id.Create<SpecialEpisode>(), this.CreateTitles(), 1, 2000 + num, false, true, "Test", num);

    protected Franchise CreateFranchise(CineasteList list)
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

    protected IEnumerable<Title> CreateTitles() =>
        [new("Test", 1, false), new("Original Test", 1, true)];

    protected MovieKind CreateMovieKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new MovieKind(Id.Create<MovieKind>(), "Test Kind", black, black, black);

        list.AddMovieKind(kind);

        return kind;
    }

    protected SeriesKind CreateSeriesKind(CineasteList list)
    {
        var black = new Color("#000000");
        var kind = new SeriesKind(Id.Create<SeriesKind>(), "Test Kind", black, black, black);

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
