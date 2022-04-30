namespace Cineaste.Core.Domain;

using System.Globalization;

public sealed class ListConfiguration : Entity<ListConfiguration>
{
    private CultureInfo culture;
    private string defaultSeasonTitle;
    private string defaultSeasonOriginalTitle;

    private readonly List<MovieKind> movieKinds;
    private readonly List<SeriesKind> seriesKinds;
    private readonly List<Tag> tags;

    public CultureInfo Culture
    {
        get => this.culture;

        [MemberNotNull(nameof(culture))]
        set => this.culture = Require.NotNull(value);
    }

    public string DefaultSeasonTitle
    {
        get => this.defaultSeasonTitle;

        [MemberNotNull(nameof(defaultSeasonTitle))]
        set => this.defaultSeasonTitle = Require.NotNull(value);
    }

    public string DefaultSeasonOriginalTitle
    {
        get => this.defaultSeasonOriginalTitle;

        [MemberNotNull(nameof(defaultSeasonOriginalTitle))]
        set => this.defaultSeasonOriginalTitle = Require.NotNull(value);
    }

    public ListSortingConfiguration SortingConfiguration { get; }

    public IReadOnlyCollection<MovieKind> MovieKinds =>
        this.movieKinds.AsReadOnly();

    public IReadOnlyCollection<SeriesKind> SeriesKinds =>
        this.seriesKinds.AsReadOnly();

    public IReadOnlyCollection<Tag> Tags =>
        this.tags.AsReadOnly();

    public ListConfiguration(
        Id<ListConfiguration> id,
        CultureInfo culture,
        string defaultSeasonTitle,
        string defaultSeasonOriginalTitle,
        ListSortingConfiguration sortingConfiguration,
        IEnumerable<MovieKind> movieKinds,
        IEnumerable<SeriesKind> seriesKinds,
        IEnumerable<Tag> tags)
        : base(id)
    {
        this.Culture = culture;
        this.DefaultSeasonTitle = defaultSeasonTitle;
        this.DefaultSeasonOriginalTitle = defaultSeasonOriginalTitle;
        this.SortingConfiguration = Require.NotNull(sortingConfiguration);

        this.movieKinds = movieKinds.ToList();
        this.seriesKinds = seriesKinds.ToList();
        this.tags = tags.ToList();
    }

    public void AddMovieKind(MovieKind movieKind) =>
        this.movieKinds.Add(movieKind);

    public void AddSeriesKind(SeriesKind seriesKind) =>
        this.seriesKinds.Add(seriesKind);

    public void AddTag(Tag tag) =>
        this.tags.Add(tag);

    public void RemoveMovieKind(MovieKind movieKind) =>
        this.movieKinds.Remove(movieKind);

    public void RemoveSeriesKind(SeriesKind seriesKind) =>
        this.seriesKinds.Remove(seriesKind);

    public void RemoveTag(Tag tag) =>
        this.tags.Remove(tag);
}
