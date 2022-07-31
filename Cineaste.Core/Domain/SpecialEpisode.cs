namespace Cineaste.Core.Domain;

public sealed class SpecialEpisode : Entity<SpecialEpisode>
{
    private int month;
    private string channel;
    private int year;
    private int sequenceNumber;
    private string? rottenTomatoesId;
    private readonly List<Title> titles;

    public int Month
    {
        get => this.month;
        set => this.month = Require.Month(value);
    }

    public int Year
    {
        get => this.year;
        set => this.year = Require.Positive(value);
    }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public string Channel
    {
        get => this.channel;

        [MemberNotNull(nameof(channel))]
        set => this.channel = Require.NotBlank(value);
    }

    public int SequenceNumber
    {
        get => this.sequenceNumber;
        set => this.sequenceNumber = Require.Positive(value);
    }

    public string? RottenTomatoesId
    {
        get => this.rottenTomatoesId;
        set => this.rottenTomatoesId = Require.RottenTomatoesId(value);
    }

    public Poster? Poster { get; set; }

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public Title Title =>
        this.Titles
            .Where(title => !title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public Title OriginalTitle =>
        this.Titles
            .Where(title => title.IsOriginal)
            .OrderBy(title => title.Priority)
            .First();

    public SpecialEpisode(
        Id<SpecialEpisode> id,
        IEnumerable<Title> titles,
        int month,
        int year,
        bool isWatched,
        bool isReleased,
        string channel,
        int sequenceNumber)
        : base(id)
    {
        this.Month = month;
        this.Year = year;
        this.IsWatched = isWatched;
        this.IsReleased = isReleased;
        this.Channel = channel;
        this.SequenceNumber = sequenceNumber;
        this.titles = titles.ToList();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private SpecialEpisode(Id<SpecialEpisode> id)
        : base(id)
    {
        this.channel = null!;
        this.titles = new();
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
