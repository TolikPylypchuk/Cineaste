namespace Cineaste.Core.Domain;

public sealed class FranchiseItem : Entity<FranchiseItem>
{
    private Franchise parentFranchise;
    private int sequenceNumber;
    private int? displayNumber;

    public Franchise ParentFranchise
    {
        get => this.parentFranchise;

        [MemberNotNull(nameof(parentFranchise))]
        set => this.parentFranchise = Require.NotNull(value);
    }

    public int SequenceNumber
    {
        get => this.sequenceNumber;
        set => this.sequenceNumber = Require.Positive(value);
    }

    public int? DisplayNumber
    {
        get => this.displayNumber;
        set => this.displayNumber = Require.Positive(value);
    }

    public Movie? Movie { get; private set; }

    public Series? Series { get; private set; }

    public Franchise? Franchise { get; private set; }

    public IReadOnlyCollection<Title> Titles =>
        this.Select(movie => movie.Titles, series => series.Titles, franchise => franchise.ActualTitles);

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

    public FranchiseItem(
        Id<FranchiseItem> id,
        Movie movie,
        Franchise parentFranchise,
        int sequenceNumber,
        int? displayNumber)
        : this(id, parentFranchise, sequenceNumber, displayNumber) =>
        this.Movie = Require.NotNull(movie);

    public FranchiseItem(
        Id<FranchiseItem> id,
        Series series,
        Franchise parentFranchise,
        int sequenceNumber,
        int? displayNumber)
        : this(id, parentFranchise, sequenceNumber, displayNumber) =>
        this.Series = Require.NotNull(series);

    public FranchiseItem(
        Id<FranchiseItem> id,
        Franchise franchise,
        Franchise parentFranchise,
        int sequenceNumber,
        int? displayNumber)
        : this(id, parentFranchise, sequenceNumber, displayNumber) =>
        this.Franchise = Require.NotNull(franchise);

    private FranchiseItem(
        Id<FranchiseItem> id,
        Franchise parentFranchise,
        int sequenceNumber,
        int? displayNumber)
        : base(id)
    {
        this.ParentFranchise = parentFranchise;
        this.SequenceNumber = sequenceNumber;
        this.DisplayNumber = displayNumber;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private FranchiseItem(Id<FranchiseItem> id)
        : base(id) =>
        this.parentFranchise = null!;
}
