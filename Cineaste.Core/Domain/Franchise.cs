namespace Cineaste.Core.Domain;

public sealed class Franchise : Entity<Franchise>
{
    private CineasteList ownerList;

    private readonly List<FranchiseItem> children;
    private readonly List<Title> titles;

    public bool ShowTitles { get; set; }
    public bool IsLooselyConnected { get; set; }
    public bool MergeDisplayNumbers { get; set; }

    public CineasteList OwnerList
    {
        get => this.ownerList;

        [MemberNotNull(nameof(ownerList))]
        set => this.ownerList = Require.NotNull(value);
    }

    public Poster? Poster { get; set; }

    public FranchiseItem? FranchiseItem { get; set; }

    public IReadOnlyCollection<FranchiseItem> Children =>
        this.children.AsReadOnly();

    public IReadOnlyCollection<Title> Titles =>
        this.titles.AsReadOnly();

    public Franchise(
        Id<Franchise> id,
        bool showTitles,
        bool isLooselyConnected,
        bool mergeDisplayNumbers,
        CineasteList ownerList,
        Poster? poster,
        FranchiseItem? franchiseItem,
        IEnumerable<FranchiseItem> children,
        IEnumerable<Title> titles)
        : base(id)
    {
        this.ShowTitles = showTitles;
        this.IsLooselyConnected = isLooselyConnected;
        this.MergeDisplayNumbers = mergeDisplayNumbers;
        this.OwnerList = ownerList;
        this.Poster = poster;
        this.FranchiseItem = franchiseItem;

        this.children = children.ToList();
        this.titles = titles.ToList();
    }
}
