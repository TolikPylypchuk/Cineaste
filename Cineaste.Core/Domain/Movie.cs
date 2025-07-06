namespace Cineaste.Core.Domain;

public sealed class Movie : FranchiseItemEntity<Movie>
{
    private int year;
    private string? imdbId;
    private string? rottenTomatoesId;
    private MovieKind kind;

    private readonly HashSet<TagContainer> tags;

    public int Year
    {
        get => this.year;
        set => this.year = Require.Positive(value);
    }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

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

    public MovieKind Kind
    {
        get => this.kind;

        [MemberNotNull(nameof(kind))]
        set => this.kind = Require.NotNull(value);
    }

    public IReadOnlySet<TagContainer> Tags =>
        this.tags;

    public Movie(
        Id<Movie> id,
        IEnumerable<Title> titles,
        int year,
        bool isWatched,
        bool isReleased,
        MovieKind kind,
        IEnumerable<Tag>? tags = null)
        : base(id, titles)
    {
        this.Year = year;
        this.IsWatched = isWatched;
        this.IsReleased = isReleased;
        this.Kind = kind;

        this.tags = [];

        if (tags != null)
        {
            foreach (var tag in tags)
            {
                this.AddTag(tag);
            }
        }
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Movie(Id<Movie> id)
        : base(id)
    {
        this.kind = null!;
        this.tags = [];
    }

    public void AddTag(Tag tag) =>
        this.tags.Add(new TagContainer { Tag = tag });

    public void RemoveTag(Tag tag) =>
        this.tags.Remove(new TagContainer { Tag = tag });
}
