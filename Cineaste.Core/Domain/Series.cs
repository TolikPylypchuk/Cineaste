namespace Cineaste.Core.Domain;

public enum SeriesWatchStatus
{
    NotWatched,
    Watching,
    Watched,
    StoppedWatching
}

public enum SeriesReleaseStatus
{
    NotStarted,
    Running,
    Finished,
    Cancelled,
    Unknown
}

public sealed class Series : Entity<Series>
{
    private string? imdbId;
    private string? rottenTomatoesLink;
    private SeriesKind kind;

    private readonly List<Title> titles;
    private readonly List<Season> seasons;
    private readonly List<SpecialEpisode> specialEpisodes;
    private readonly HashSet<Tag> tags;

    public bool IsMiniseries { get; set; }

    public SeriesWatchStatus WatchStatus { get; set; } = SeriesWatchStatus.NotWatched;
    public SeriesReleaseStatus ReleaseStatus { get; set; } = SeriesReleaseStatus.NotStarted;

    public SeriesKind Kind
    {
        get => this.kind;

        [MemberNotNull(nameof(kind))]
        set => this.kind = Require.NotNull(value);
    }

    public string? ImdbId
    {
        get => this.imdbId;
        set => this.imdbId = Require.ImdbId(value);
    }

    public string? RottenTomatoesLink
    {
        get => this.rottenTomatoesLink;
        set => this.rottenTomatoesLink = Require.Url(value);
    }

    public Poster? Poster { get; set; }

    public FranchiseItem? FranchiseItem { get; set; }

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public IReadOnlyCollection<Season> Seasons =>
        this.seasons.AsReadOnly();

    public IReadOnlyCollection<SpecialEpisode> SpecialEpisodes =>
        this.specialEpisodes.AsReadOnly();

    public IReadOnlySet<Tag> Tags =>
        this.tags;

    public Series(
        Id<Series> id,
        bool isMiniseries,
        SeriesWatchStatus watchStatus,
        SeriesReleaseStatus releaseStatus,
        SeriesKind kind,
        string? imdbId,
        string? rottenTomatoesLink,
        Poster? poster,
        FranchiseItem? franchiseItem,
        IEnumerable<Title> titles,
        IEnumerable<Season> seasons,
        IEnumerable<SpecialEpisode> specialEpisodes,
        IEnumerable<Tag> tags)
        : base(id)
    {
        this.IsMiniseries = isMiniseries;
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Kind = kind;
        this.ImdbId = imdbId;
        this.RottenTomatoesLink = rottenTomatoesLink;
        this.Poster = poster;
        this.FranchiseItem = franchiseItem;

        this.titles = titles.ToList();
        this.seasons = seasons.ToList();
        this.specialEpisodes = specialEpisodes.ToList();
        this.tags = tags.ToHashSet();
    }
}
