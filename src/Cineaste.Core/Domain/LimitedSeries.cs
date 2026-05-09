namespace Cineaste.Core.Domain;

public sealed class LimitedSeries : FranchiseItemEntity<LimitedSeries>
{
    private string channel;
    private SeriesKind kind;

    private readonly HashSet<TagContainer> tags;

    public ReleasePeriod Period { get; set; }

    public SeriesWatchStatus WatchStatus
    {
        get;
        set => field = Require.ValidEnum(value);
    }

    public SeriesReleaseStatus ReleaseStatus
    {
        get;
        set => field = Require.ValidEnum(value);
    }

    public string Channel
    {
        get => this.channel;

        [MemberNotNull(nameof(channel))]
        set => this.channel = Require.NotBlank(value);
    }

    public SeriesKind Kind
    {
        get => this.kind;

        [MemberNotNull(nameof(kind))]
        set => this.kind = Require.NotNull(value);
    }

    public ImdbId? ImdbId { get; set; }

    public RottenTomatoesId? RottenTomatoesId { get; set; }

    public RottenTomatoesId? RottenTomatoesSubId { get; set; }

    public PosterHash? PosterHash { get; set; }

    public IReadOnlySet<TagContainer> Tags =>
        this.tags;

    public LimitedSeries(
        Id<LimitedSeries> id,
        IEnumerable<Title> titles,
        ReleasePeriod period,
        SeriesWatchStatus watchStatus,
        SeriesReleaseStatus releaseStatus,
        string channel,
        SeriesKind kind)
        : base(id, titles)
    {
        this.Period = period;
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Channel = channel;
        this.Kind = kind;

        this.tags = [];
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private LimitedSeries(Id<LimitedSeries> id)
        : base(id)
    {
        this.channel = null!;
        this.kind = null!;
        this.tags = [];
    }

    public void AddTag(Tag tag) =>
        this.tags.Add(new TagContainer { Tag = tag });

    public void RemoveTag(Tag tag) =>
        this.tags.Remove(new TagContainer { Tag = tag });
}
