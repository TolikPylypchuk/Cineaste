namespace Cineaste.Core.Domain;

public sealed class Franchise : Entity<Franchise>
{
    private readonly List<FranchiseItem> children;
    private readonly List<Title> titles;

    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool MergeDisplayNumbers { get; set; }

    public Poster? Poster { get; set; }

    public FranchiseItem? FranchiseItem { get; set; }

    public IReadOnlyCollection<FranchiseItem> Children =>
        this.children.AsReadOnly();

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public Franchise(
        Id<Franchise> id,
        IEnumerable<Title> titles,
        IEnumerable<FranchiseItem> children,
        bool showTitles,
        bool isLooselyConnected,
        bool mergeDisplayNumbers)
        : base(id)
    {
        this.ShowTitles = showTitles;
        this.IsLooselyConnected = isLooselyConnected;
        this.MergeDisplayNumbers = mergeDisplayNumbers;

        this.children = children.ToList();
        this.titles = titles.ToList();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private Franchise(Id<Franchise> id)
        : base(id)
    {
        this.titles = new();
        this.children = new();
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
