namespace Cineaste.Core.Domain;

public sealed class FranchiseItem : Entity<FranchiseItem>
{
    private Franchise parentFranchise;
    private int sequenceNumber;

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

    public bool ShouldDisplayNumber { get; set; }

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
        bool shouldDisplayNumber)
        : this(id, parentFranchise, sequenceNumber, shouldDisplayNumber) =>
        this.Movie = Require.NotNull(movie);

    public FranchiseItem(
        Id<FranchiseItem> id,
        Series series,
        Franchise parentFranchise,
        int sequenceNumber,
        bool shouldDisplayNumber)
        : this(id, parentFranchise, sequenceNumber, shouldDisplayNumber) =>
        this.Series = Require.NotNull(series);

    public FranchiseItem(
        Id<FranchiseItem> id,
        Franchise franchise,
        Franchise parentFranchise,
        int sequenceNumber,
        bool shouldDisplayNumber)
        : this(id, parentFranchise, sequenceNumber, shouldDisplayNumber) =>
        this.Franchise = Require.NotNull(franchise);

    private FranchiseItem(
        Id<FranchiseItem> id,
        Franchise parentFranchise,
        int sequenceNumber,
        bool shouldDisplayNumber)
        : base(id)
    {
        this.ParentFranchise = parentFranchise;
        this.SequenceNumber = sequenceNumber;
        this.ShouldDisplayNumber = shouldDisplayNumber;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private FranchiseItem(Id<FranchiseItem> id)
        : base(id) =>
        this.parentFranchise = null!;

    public T Select<T>(Func<Movie, T> movieFunc, Func<Series, T> seriesFunc, Func<Franchise, T> franchiseFunc)
    {
        if (this.Movie is not null)
        {
            return movieFunc(this.Movie);
        } else if (this.Series is not null)
        {
            return seriesFunc(this.Series);
        } else if (this.Franchise is not null)
        {
            return franchiseFunc(this.Franchise);
        } else
        {
            throw new InvalidOperationException("Exactly one franchise item component must be non-null");
        }
    }

    public void Do(Action<Movie> movieAction, Action<Series> seriesAction, Action<Franchise> franchiseAction)
    {
        if (this.Movie is not null)
        {
            movieAction(this.Movie);
        } else if (this.Series is not null)
        {
            seriesAction(this.Series);
        } else if (this.Franchise is not null)
        {
            franchiseAction(this.Franchise);
        } else
        {
            throw new InvalidOperationException("Exactly one franchise item component must be non-null");
        }
    }
}
