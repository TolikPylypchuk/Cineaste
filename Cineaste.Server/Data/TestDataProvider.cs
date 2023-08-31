namespace Cineaste.Server.Data;

using System.ComponentModel;
using System.Globalization;

internal sealed class TestDataProvider
{
    private readonly CineasteDbContext dbContext;

    public TestDataProvider(CineasteDbContext dbContext) =>
        this.dbContext = dbContext;

    public async Task CreateTestDataIfNeeded()
    {
        bool listsPresentInDb = await this.dbContext.Lists.AnyAsync();

        if (!listsPresentInDb)
        {
            this.dbContext.Lists.Add(this.CreateList());
            await this.dbContext.SaveChangesAsync();
        }
    }

    private CineasteList CreateList()
    {
        var config = new ListConfiguration(
            Id.Create<ListConfiguration>(),
            CultureInfo.InvariantCulture,
            "Season #",
            "Season #",
            new(ListSortOrder.ByTitle, ListSortDirection.Ascending, ListSortOrder.ByYear, ListSortDirection.Ascending));

        var list = new CineasteList(Id.Create<CineasteList>(), config);

        var black = new Color("#000000");
        var blue = new Color("#0000FF");

        var movieKind = new MovieKind(Id.Create<MovieKind>(), "Test Kind", black, black, black);
        var seriesKind = new SeriesKind(Id.Create<SeriesKind>(), "Test Kind", blue, blue, blue);

        list.AddMovieKind(movieKind);
        list.AddSeriesKind(seriesKind);

        var a = this.CreateMovie("a", movieKind);
        var b = this.CreateMovie("b", movieKind);
        var c = this.CreateMovie("c", movieKind);
        var d = this.CreateMovie("d", movieKind);
        var e = this.CreateMovie("e", movieKind);

        var f = this.CreateSeries("f", seriesKind);
        var g = this.CreateSeries("g", seriesKind);
        var h = this.CreateSeries("h", seriesKind);

        var i = this.CreateFranchise("i");

        i.AddSeries(f);
        i.AddMovie(c);

        list.AddMovie(a);
        list.AddMovie(b);
        list.AddMovie(c);
        list.AddMovie(d);
        list.AddMovie(e);

        list.AddSeries(f);
        list.AddSeries(g);
        list.AddSeries(h);

        list.AddFranchise(i);

        return list;
    }

    private Movie CreateMovie(string title, MovieKind kind) =>
        new(
            Id.Create<Movie>(),
            new List<Title> { new(title, 1, false), new(title, 1, true) },
            DateTime.Now.Year,
            isWatched: true,
            isReleased: true,
            kind);

    private Series CreateSeries(string title, SeriesKind kind)
    {
        const string channel = "HBO";

        var period1 = new Period(Id.Create<Period>(), 1, 2000, 2, 2000, false, 10);

        var season1 = new Season(
            Id.Create<Season>(),
            new List<Title> { new("Season 1", 1, false), new("Season 1", 1, true) },
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            new List<Period> { period1 });

        var episode = new SpecialEpisode(
            Id.Create<SpecialEpisode>(),
            new List<Title> { new("Special Episode", 1, false), new("Special Episode", 1, true) },
            month: 5,
            year: 2001,
            isWatched: true,
            isReleased: true,
            channel,
            sequenceNumber: 2);

        var period2 = new Period(Id.Create<Period>(), 1, 2002, 2, 2002, false, 10);

        var season2 = new Season(
            Id.Create<Season>(),
            new List<Title> { new("Season 2", 1, false), new("Season 2", 1, true) },
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 3,
            new List<Period> { period2 });

        return new(
            Id.Create<Series>(),
            new List<Title> { new(title, 1, false), new(title, 1, true) },
            new List<Season> { season1, season2 },
            new List<SpecialEpisode> { episode },
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            kind);
    }

    private Franchise CreateFranchise(string? title = null) =>
        new(
            Id.Create<Franchise>(),
            title is not null ? new List<Title> { new(title, 1, false), new(title, 1, true) } : new List<Title>(),
            showTitles: title is not null,
            isLooselyConnected: false,
            continueNumbering: false);
}
