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
            this.CreateList("Test List 1"),
            this.CreateList("Test List 2"),
            this.CreateList("Test List 3"),
            this.CreateList("Test List 4"),
            this.CreateList("Test List 5"));

        await this.dbContext.SaveChangesAsync();
    }

    private CineasteList CreateList(string name)
    {
        var config = new ListConfiguration(
            Id.Create<ListConfiguration>(),
            CultureInfo.InvariantCulture,
            "Season #",
            "Season #",
            new(ListSortOrder.ByTitle, ListSortDirection.Ascending, ListSortOrder.ByYear, ListSortDirection.Ascending));

        var list = new CineasteList(Id.Create<CineasteList>(), name, config);

        var black = new Color("#00000000");

        var movieKind = new MovieKind(Id.Create<MovieKind>(), "Test Kind", black, black, black);
        var seriesKind = new SeriesKind(Id.Create<SeriesKind>(), "Test Kind", black, black, black);

        list.AddMovieKind(movieKind);
        list.AddSeriesKind(seriesKind);

        var a = this.CreateMovie("a", movieKind);
        var b = this.CreateMovie("b", movieKind);
        var c = this.CreateMovie("c", movieKind);

        list.AddMovie(a);
        list.AddMovie(b);
        list.AddMovie(c);

        return list;
    }

    private Movie CreateMovie(string title, MovieKind kind, FranchiseItem? franchiseItem = null) =>
        new(
            Id.Create<Movie>(),
            new List<Title> { new(title, 1, false), new(title, 1, true) },
            DateTime.Now.Year,
            isWatched: true,
            isReleased: true,
            kind)
        {
            FranchiseItem = franchiseItem
        };
}
