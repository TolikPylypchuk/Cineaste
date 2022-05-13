namespace Cineaste.Core.Domain;

public sealed class Movie : Entity<Movie>
{
    private int year;
    private string? imdbId;
    private string? rottenTomatoesLink;
    private MovieKind kind;

    private readonly List<Title> titles;
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

    public string? RottenTomatoesLink
    {
        get => this.rottenTomatoesLink;
        set => this.rottenTomatoesLink = Require.Url(value);
    }

    public Poster? Poster { get; set; }

    public MovieKind Kind
    {
        get => this.kind;

        [MemberNotNull(nameof(kind))]
        set => this.kind = Require.NotNull(value);
    }

    public FranchiseItem? FranchiseItem { get; set; }

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

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
        : base(id)
    {
        this.Year = year;
        this.IsWatched = isWatched;
        this.IsReleased = isReleased;
        this.Kind = kind;

        this.titles = titles.ToList();
        this.tags = new();

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
        this.titles = new();
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
