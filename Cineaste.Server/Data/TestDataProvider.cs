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
        int listsCount = await this.dbContext.Lists.CountAsync();

        if (listsCount != 0)
        {
            return;
        }

        this.dbContext.Lists.AddRange(
            this.CreateList("Test List 1", "test-list-1"),
            this.CreateList("Test List 2", "test-list-2"),
            this.CreateList("Test List 3", "test-list-3"),
            this.CreateList("Test List 4", "test-list-4"),
            this.CreateList("Test List 5", "test-list-5"));

        await this.dbContext.SaveChangesAsync();
    }

    private CineasteList CreateList(string name, string handle)
    {
        var config = new ListConfiguration(
            Id.CreateNew<ListConfiguration>(),
            CultureInfo.InvariantCulture,
            "Season #",
            "Season #",
            new(ListSortOrder.ByTitle, ListSortDirection.Ascending, ListSortOrder.ByYear, ListSortDirection.Ascending));

        var list = new CineasteList(Id.CreateNew<CineasteList>(), name, handle, config);

        var black = new Color("#000000");
        var blue = new Color("#0000FF");

        var movieKind = new MovieKind(Id.CreateNew<MovieKind>(), "Test Kind", black, black, black);
        var seriesKind = new SeriesKind(Id.CreateNew<SeriesKind>(), "Test Kind", blue, blue, blue);

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
            Id.CreateNew<Movie>(),
            new List<Title> { new(title, 1, false), new(title, 1, true) },
            DateTime.Now.Year,
            isWatched: true,
            isReleased: true,
            kind);

    private Series CreateSeries(string title, SeriesKind kind)
    {
        const string channel = "HBO";

        var period1 = new Period(Id.CreateNew<Period>(), 1, 2000, 2, 2000, false, 10);

        var season1 = new Season(
            Id.CreateNew<Season>(),
            new List<Title> { new("Season 1", 1, false), new("Season 2", 1, true) },
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 1,
            new List<Period> { period1 });

        var episode = new SpecialEpisode(
            Id.CreateNew<SpecialEpisode>(),
            new List<Title> { new("Special Episode", 1, false), new("Special Episode", 1, true) },
            month: 5,
            year: 2001,
            isWatched: true,
            isReleased: true,
            channel,
            sequenceNumber: 2);

        var period2 = new Period(Id.CreateNew<Period>(), 1, 2002, 2, 2002, false, 10);

        var season2 = new Season(
            Id.CreateNew<Season>(),
            new List<Title> { new("Season 2", 1, false), new("Season 2", 1, true) },
            SeasonWatchStatus.Watched,
            SeasonReleaseStatus.Finished,
            channel,
            sequenceNumber: 3,
            new List<Period> { period2 });

        return new(
            Id.CreateNew<Series>(),
            new List<Title> { new(title, 1, false), new(title, 1, true) },
            new List<Season> { season1, season2 },
            new List<SpecialEpisode> { episode },
            isMiniseries: false,
            SeriesWatchStatus.Watched,
            SeriesReleaseStatus.Finished,
            kind);
    }

    private Franchise CreateFranchise(string? title = null) =>
        new(
            Id.CreateNew<Franchise>(),
            title is not null ? new List<Title> { new(title, 1, false), new(title, 1, true) } : new List<Title>(),
            new List<FranchiseItem>(),
            showTitles: title is not null,
            isLooselyConnected: false,
            mergeDisplayNumbers: false);
}
