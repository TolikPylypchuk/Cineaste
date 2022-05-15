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
    private readonly HashSet<TagContainer> tags;

    public bool IsMiniseries { get; set; }

    public SeriesWatchStatus WatchStatus { get; set; }
    public SeriesReleaseStatus ReleaseStatus { get; set; }

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

    public IReadOnlySet<TagContainer> Tags =>
        this.tags;

    public Series(
        Id<Series> id,
        IEnumerable<Title> titles,
        IEnumerable<Season> seasons,
        IEnumerable<SpecialEpisode> specialEpisodes,
        bool isMiniseries,
        SeriesWatchStatus watchStatus,
        SeriesReleaseStatus releaseStatus,
        SeriesKind kind,
        IEnumerable<TagContainer> tags)
        : base(id)
    {
        this.IsMiniseries = isMiniseries;
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Kind = kind;

        this.titles = titles.ToList();
        this.seasons = seasons.ToList();
        this.specialEpisodes = specialEpisodes.ToList();
        this.tags = tags.ToHashSet();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Series(Id<Series> id)
        : base(id)
    {
        this.kind = null!;
        this.titles = new();
        this.seasons = new();
        this.specialEpisodes = new();
        this.tags = new();
    }

    public Title AddTitle(string name, bool isOriginal)
    {
        int priority = this.titles
            .Where(title => title.IsOriginal == isOriginal)
            .Max(title => title.Priority) + 1;

        var title = new Title(name, priority, isOriginal);

        this.titles.Add(title);

        return title;
    }

    public void RemoveTitle(string name, bool isOriginal) =>
        this.titles.RemoveAll(title => title.Name == name && title.IsOriginal == isOriginal);

    public void AddTag(Tag tag) =>
        this.tags.Add(new TagContainer { Tag = tag });

    public void RemoveTag(Tag tag) =>
        this.tags.Remove(new TagContainer { Tag = tag });
}