using System.ComponentModel;
using System.Globalization;

namespace Cineaste.Core.Domain;

public sealed class CineasteList : Entity<CineasteList>
{
    private readonly List<ListItem> items;

    private readonly List<MovieKind> movieKinds;
    private readonly List<SeriesKind> seriesKinds;
    private readonly List<Tag> tags;

    public ListConfiguration Configuration { get; private set; }

    public IReadOnlyCollection<ListItem> Items =>
        this.items.AsReadOnly();

    public IReadOnlyCollection<MovieKind> MovieKinds =>
        this.movieKinds.AsReadOnly();

    public IReadOnlyCollection<SeriesKind> SeriesKinds =>
        this.seriesKinds.AsReadOnly();

    public IReadOnlyCollection<Tag> Tags =>
        this.tags.AsReadOnly();

    public CineasteList(Id<CineasteList> id, ListConfiguration configuration)
        : base(id)
    {
        this.Configuration = Require.NotNull(configuration);

        this.items = [];

        this.movieKinds = [];
        this.seriesKinds = [];
        this.tags = [];
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private CineasteList(Id<CineasteList> id)
        : base(id)
    {
        this.Configuration = null!;

        this.items = [];

        this.movieKinds = [];
        this.seriesKinds = [];
        this.tags = [];
    }

    public void AddItem(ListItem item) =>
        this.items.Add(item);

    public void RemoveItem(ListItem item) =>
        this.items.Remove(item);

    public void AddMovie(Movie movie) =>
        this.items.Add(new ListItem(Domain.Id.Create<ListItem>(), this, movie));

    public void RemoveMovie(Movie movie) =>
        this.items.RemoveAll(item => item is { Movie.Id: var id } && id == movie.Id);

    public void AddSeries(Series series) =>
        this.items.Add(new ListItem(Domain.Id.Create<ListItem>(), this, series));

    public void RemoveSeries(Series series) =>
        this.items.RemoveAll(item => item is { Series.Id: var id } && id == series.Id);

    public void AddFranchise(Franchise franchise) =>
        this.items.Add(new ListItem(Domain.Id.Create<ListItem>(), this, franchise));

    public void RemoveFranchise(Franchise franchise) =>
        this.items.RemoveAll(item => item is { Franchise.Id: var id } && id == franchise.Id);

    public void AddMovieKind(MovieKind movieKind) =>
        this.movieKinds.Add(movieKind);

    public void RemoveMovieKind(MovieKind movieKind) =>
        this.movieKinds.Remove(movieKind);

    public void AddSeriesKind(SeriesKind seriesKind) =>
        this.seriesKinds.Add(seriesKind);

    public void RemoveSeriesKind(SeriesKind seriesKind) =>
        this.seriesKinds.Remove(seriesKind);

    public void AddTag(Tag tag) =>
        this.tags.Add(tag);

    public void RemoveTag(Tag tag) =>
        this.tags.Remove(tag);

    public bool ContainsMovie(Movie movie) =>
        this.items.Any(item => item is { Movie.Id: var id } && id == movie.Id);

    public bool ContainsSeries(Series series) =>
        this.items.Any(item => item is { Series.Id: var id } && id == series.Id);

    public bool ContainsFranchise(Franchise franchise) =>
        this.items.Any(item => item is { Franchise.Id: var id } && id == franchise.Id);

    public void SortItems()
    {
        bool ascending = this.Configuration.SortingConfiguration.SortDirection == ListSortDirection.Ascending;
        bool secondOrderOriginal =
            this.Configuration.SortingConfiguration.SecondSortOrder == ListSortOrder.ByOriginalTitleSimple;

        var comparer = this.Configuration.Culture.CompareInfo.GetStringComparer(CompareOptions.IgnoreCase);

        var orderedItems = this.Configuration.SortingConfiguration.FirstSortOrder switch
        {
            ListSortOrder.ByTitle => ascending
                ? this.items
                    .OrderBy(item => item.NormalizedTitle, comparer)
                    .ThenBy(item => item.StartYear)
                    .ThenBy(item => item.EndYear)
                : this.items
                    .OrderByDescending(item => item.NormalizedTitle, comparer)
                    .ThenByDescending(item => item.StartYear)
                    .ThenByDescending(item => item.EndYear),
            ListSortOrder.ByOriginalTitle => ascending
                ? this.items
                    .OrderBy(item => item.NormalizedOriginalTitle, comparer)
                    .ThenBy(item => item.StartYear)
                    .ThenBy(item => item.EndYear)
                : this.items
                    .OrderByDescending(item => item.NormalizedOriginalTitle, comparer)
                    .ThenByDescending(item => item.StartYear)
                    .ThenByDescending(item => item.EndYear),
            ListSortOrder.ByTitleSimple => ascending
                ? this.items
                    .OrderBy(item => item.NormalizedShortTitle, comparer)
                    .ThenBy(item => item.StartYear)
                    .ThenBy(item => item.EndYear)
                : this.items
                    .OrderByDescending(item => item.NormalizedShortTitle, comparer)
                    .ThenByDescending(item => item.StartYear)
                    .ThenByDescending(item => item.EndYear),
            ListSortOrder.ByOriginalTitleSimple => ascending
                ? this.items
                    .OrderBy(item => item.NormalizedShortOriginalTitle, comparer)
                    .ThenBy(item => item.StartYear)
                    .ThenBy(item => item.EndYear)
                : this.items
                    .OrderByDescending(item => item.NormalizedShortOriginalTitle, comparer)
                    .ThenByDescending(item => item.StartYear)
                    .ThenByDescending(item => item.EndYear),
            ListSortOrder.ByYear => ascending
                ? secondOrderOriginal
                    ? this.items
                        .OrderBy(item => item.StartYear)
                        .ThenBy(item => item.EndYear)
                        .ThenBy(item => item.NormalizedShortTitle, comparer)
                    : this.items
                        .OrderBy(item => item.StartYear)
                        .ThenBy(item => item.EndYear)
                        .ThenBy(item => item.NormalizedShortOriginalTitle, comparer)
                : secondOrderOriginal
                    ? this.items
                        .OrderByDescending(item => item.StartYear)
                        .ThenByDescending(item => item.EndYear)
                        .ThenByDescending(item => item.NormalizedShortTitle, comparer)
                    : this.items
                        .OrderByDescending(item => item.StartYear)
                        .ThenByDescending(item => item.EndYear)
                        .ThenByDescending(item => item.NormalizedShortOriginalTitle, comparer),
            _ => this.items.AsEnumerable()
        };

        var newItems = orderedItems.ToList();

        this.items.Clear();
        this.items.AddRange(newItems);

        int sequenceNumber = 1;
        foreach (var item in this.items)
        {
            item.SequenceNumber = sequenceNumber++;
        }
    }
}
