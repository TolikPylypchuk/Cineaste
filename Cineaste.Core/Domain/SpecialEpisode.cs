namespace Cineaste.Core.Domain;

public sealed class SpecialEpisode : Entity<SpecialEpisode>
{
    private string channel;
    private string? rottenTomatoesLink;

    private readonly List<Title> titles;

    public int Month { get; set; }
    public int Year { get; set; }

    public bool IsWatched { get; set; }
    public bool IsReleased { get; set; }

    public string Channel
    {
        get => channel;

        [MemberNotNull(nameof(channel))]
        set => channel = Require.NotBlank(value);
    }

    public int SequenceNumber { get; set; }

    public string? RottenTomatoesLink
    {
        get => this.rottenTomatoesLink;
        set => this.rottenTomatoesLink = Require.Url(value);
    }

    public Poster? Poster { get; set; }

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public SpecialEpisode(
        Id<SpecialEpisode> id,
        int month,
        int year,
        bool isWatched,
        bool isReleased,
        string channel,
        int sequenceNumber,
        string? rottenTomatoesLink,
        Poster? poster,
        IEnumerable<Title> titles)
        : base(id)
    {
        this.Month = month;
        this.Year = year;
        this.IsWatched = isWatched;
        this.IsReleased = isReleased;
        this.Channel = channel;
        this.SequenceNumber = sequenceNumber;
        this.RottenTomatoesLink = rottenTomatoesLink;
        this.Poster = poster;
        this.titles = titles.ToList();
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
