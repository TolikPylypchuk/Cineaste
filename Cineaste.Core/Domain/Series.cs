namespace Cineaste.Core.Domain;

public sealed class Series : FranchiseItemEntity<Series>
{
    private string? imdbId;
    private string? rottenTomatoesId;
    private SeriesKind kind;

    private readonly List<Season> seasons;
    private readonly List<SpecialEpisode> specialEpisodes;
    private readonly HashSet<TagContainer> tags;

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

    public string? RottenTomatoesId
    {
        get => this.rottenTomatoesId;
        set => this.rottenTomatoesId = Require.RottenTomatoesId(value);
    }

    public Poster? Poster { get; set; }

    public IReadOnlyCollection<Season> Seasons =>
        this.seasons.AsReadOnly();

    public IReadOnlyCollection<SpecialEpisode> SpecialEpisodes =>
        this.specialEpisodes.AsReadOnly();

    public IReadOnlySet<TagContainer> Tags =>
        this.tags;

    public int StartYear =>
        Math.Min(
            this.Seasons
                .OrderBy(season => season.StartYear)
                .FirstOrDefault()
                ?.StartYear
                ?? Int32.MaxValue,
            this.SpecialEpisodes
                .OrderBy(episode => episode.Year)
                .ThenBy(episode => episode.Month)
                .FirstOrDefault()
                ?.Year
                ?? Int32.MaxValue);

    public int EndYear =>
        Math.Max(
            this.Seasons
                .OrderByDescending(season => season.EndYear)
                .FirstOrDefault()
                ?.EndYear
                ?? Int32.MinValue,
            this.SpecialEpisodes
                .OrderByDescending(episode => episode.Year)
                .ThenByDescending(episode => episode.Month)
                .FirstOrDefault()
                ?.Year
                ?? Int32.MinValue);

    public Series(
        Id<Series> id,
        IEnumerable<Title> titles,
        IEnumerable<Season> seasons,
        IEnumerable<SpecialEpisode> specialEpisodes,
        SeriesWatchStatus watchStatus,
        SeriesReleaseStatus releaseStatus,
        SeriesKind kind)
        : base(id, titles)
    {
        this.WatchStatus = watchStatus;
        this.ReleaseStatus = releaseStatus;
        this.Kind = kind;

        this.seasons = seasons.ToList();
        this.specialEpisodes = specialEpisodes.ToList();
        this.tags = [];
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Series(Id<Series> id)
        : base(id)
    {
        this.kind = null!;
        this.seasons = [];
        this.specialEpisodes = [];
        this.tags = [];
    }

    public void AddTag(Tag tag) =>
        this.tags.Add(new TagContainer { Tag = tag });

    public void RemoveTag(Tag tag) =>
        this.tags.Remove(new TagContainer { Tag = tag });

    public void AddSeason(Season season) =>
        this.seasons.Add(season);

    public void RemoveSeason(Season season) =>
        this.seasons.Remove(season);

    public void AddSpecialEpisode(SpecialEpisode episode) =>
        this.specialEpisodes.Add(episode);

    public void RemoveSpecialEpisode(SpecialEpisode episode) =>
        this.specialEpisodes.Remove(episode);
}
