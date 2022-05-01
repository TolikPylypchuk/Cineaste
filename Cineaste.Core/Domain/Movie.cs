namespace Cineaste.Core.Domain;

public sealed class Movie : Entity<Movie>
{
    private int year;
    private string? imdbId;
    private string? rottenTomatoesLink;
    private MovieKind kind;

    private readonly List<Title> titles;
    private readonly HashSet<Tag> tags;

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

    public CineasteList OwnerList { get; }

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public IReadOnlySet<Tag> Tags =>
        this.tags;

    public Movie(
        Id<Movie> id,
        int year,
        bool isWatched,
        bool isReleased,
        string? imdbId,
        string? rottenTomatoesLink,
        Poster? poster,
        MovieKind kind,
        FranchiseItem? franchiseItem,
        CineasteList ownerList,
        IEnumerable<Title> titles,
        IEnumerable<Tag> tags)
        : base(id)
    {
        this.Year = year;
        this.IsWatched = isWatched;
        this.IsReleased = isReleased;
        this.ImdbId = imdbId;
        this.RottenTomatoesLink = rottenTomatoesLink;
        this.Poster = poster;
        this.Kind = kind;
        this.FranchiseItem = franchiseItem;
        this.OwnerList = ownerList;

        this.titles = titles.ToList();
        this.tags = tags.ToHashSet();
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
}
