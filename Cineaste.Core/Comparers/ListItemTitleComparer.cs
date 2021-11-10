namespace Cineaste.Core.Comparers;

public sealed class ListItemTitleComparer : NullableComparerBase<ListItem>
{
    private readonly TitleComparer titleComparer;
    private readonly IComparer<ListItem> otherComparer;
    private readonly IComparer<ListItem> titleAndOtherComparer;
    private readonly Func<ListItem, string> getTitle;
    private readonly Func<Franchise, string> getFranchiseTitle;

    public ListItemTitleComparer(
        CultureInfo culture,
        IComparer<ListItem> otherComparer,
        Func<ListItem, string> getTitle,
        Func<Franchise, string> getFranchiseTitle,
        NullComparison nullComparison = NullComparison.NullsFirst)
        : base(nullComparison)
    {
        this.getTitle = getTitle;
        this.getFranchiseTitle = getFranchiseTitle;

        this.titleComparer = new TitleComparer(culture);
        this.otherComparer = otherComparer;
        this.titleAndOtherComparer = ComparerBuilder.For<ListItem>()
            .OrderBy(this.getTitle, this.titleComparer)
            .ThenBy(otherComparer);
    }

    protected override bool EqualsSafe(ListItem x, ListItem y) =>
        x == y;

    protected override int GetHashCodeSafe(ListItem item) =>
        item.GetHashCode();

    protected override int CompareSafe(ListItem x, ListItem y) =>
        (x, y) switch
        {
            (MovieListItem left, MovieListItem right) => this.Compare(left, right),
            (MovieListItem left, SeriesListItem right) => this.Compare(left, right),
            (MovieListItem left, FranchiseListItem right) => this.Compare(left, right),

            (SeriesListItem left, MovieListItem right) => this.Compare(left, right),
            (SeriesListItem left, SeriesListItem right) => this.Compare(left, right),
            (SeriesListItem left, FranchiseListItem right) => this.Compare(left, right),

            (FranchiseListItem left, MovieListItem right) => this.Compare(left, right),
            (FranchiseListItem left, SeriesListItem right) => this.Compare(left, right),
            (FranchiseListItem left, FranchiseListItem right) => this.Compare(left, right),

            _ => throw new NotSupportedException(
                $"Types of list items to compare are not supported: {x.GetType()}, {y.GetType()}")
        };

    private int Compare(MovieListItem left, MovieListItem right) =>
        left.Movie.Id == right.Movie.Id
            ? 0
            : this.CompareEntries(left, right, left.Movie.Entry, right.Movie.Entry);

    private int Compare(MovieListItem left, SeriesListItem right) =>
        this.CompareEntries(left, right, left.Movie.Entry, right.Series.Entry);

    private int Compare(MovieListItem left, FranchiseListItem right) =>
        this.Compare(right, left) * -1;

    private int Compare(SeriesListItem left, MovieListItem right) =>
        this.CompareEntries(left, right, left.Series.Entry, right.Movie.Entry);

    private int Compare(SeriesListItem left, SeriesListItem right) =>
        left.Series.Id == right.Series.Id
            ? 0
            : this.CompareEntries(left, right, left.Series.Entry, right.Series.Entry);

    private int Compare(SeriesListItem left, FranchiseListItem right) =>
        this.Compare(right, left) * -1;

    private int Compare(FranchiseListItem left, MovieListItem right) =>
        this.CompareEntries(left, right, right.Movie.Entry);

    private int Compare(FranchiseListItem left, SeriesListItem right) =>
        this.CompareEntries(left, right, right.Series.Entry);

    private int Compare(FranchiseListItem left, FranchiseListItem right)
    {
        int result;

        if (left.Franchise.Id == right.Franchise.Id)
        {
            result = 0;
        } else if (left.Franchise.IsDescendantOf(right.Franchise))
        {
            result = 1;
        } else if (right.Franchise.IsDescendantOf(left.Franchise))
        {
            result = -1;
        } else if (left.Franchise.GetRootSeries().Id == right.Franchise.GetRootSeries().Id)
        {
            var (ancestor1, ancestor2) = left.Franchise.GetDistinctAncestors(right.Franchise);
            result = ancestor1.Entry?.SequenceNumber.CompareTo(ancestor2.Entry?.SequenceNumber) ?? 0;
        } else if (left.Franchise.Entry == null && right.Franchise.Entry == null)
        {
            result = this.titleComparer.Compare(
                this.getFranchiseTitle(left.Franchise),
                this.getFranchiseTitle(right.Franchise));

            if (result == 0)
            {
                result = this.otherComparer.Compare(left, right);
            }
        } else
        {
            return this.Compare(
                new FranchiseListItem(left.Franchise.GetRootSeries()),
                new FranchiseListItem(right.Franchise.GetRootSeries()));
        }

        return result;
    }

    private int CompareEntries(
        ListItem left,
        ListItem right,
        FranchiseEntry? leftEntry,
        FranchiseEntry? rightEntry) =>
        (leftEntry, rightEntry) switch
        {
            (null, null) => this.CompareTitle(left, right),
            (var entry, null) => this.Compare(new FranchiseListItem(entry.ParentFranchise.GetRootSeries()), right),
            (null, var entry) => this.Compare(left, new FranchiseListItem(entry.ParentFranchise.GetRootSeries())),
            var (entry1, entry2) => entry1.ParentFranchise.Id == entry2.ParentFranchise.Id
                ? entry1.SequenceNumber.CompareTo(entry2.SequenceNumber)
                : this.CompareEntries(entry1, entry2)
        };

    private int CompareEntries(FranchiseListItem left, ListItem right, FranchiseEntry? entry)
    {
        if (entry == null)
        {
            return this.CompareTitle(new FranchiseListItem(left.Franchise.GetRootSeries()), right);
        }

        if (left.Franchise.Id == entry.ParentFranchise.Id)
        {
            return -1;
        }

        if (left.Franchise.IsStrictDescendantOf(entry.ParentFranchise))
        {
            return left.Franchise.GetAllAncestors()
                    .Where(f => f.Entry != null)
                    .First(f => f.Entry!.ParentFranchise.Id == entry.ParentFranchise.Id)
                    .Entry!
                .SequenceNumber
                .CompareTo(entry.SequenceNumber);
        }

        return this.Compare(left, new FranchiseListItem(entry.ParentFranchise));
    }

    private int CompareEntries(FranchiseEntry left, FranchiseEntry right)
    {
        if (left.ParentFranchise.IsStrictDescendantOf(right.ParentFranchise))
        {
            return left.ParentFranchise.GetAllAncestors()
                    .Where(a => a.Entry != null)
                    .First(a => a.Entry!.ParentFranchise.Id == right.ParentFranchise.Id)
                    .Entry!
                .SequenceNumber
                .CompareTo(right.SequenceNumber);
        }

        if (right.ParentFranchise.IsStrictDescendantOf(left.ParentFranchise))
        {
            return left.SequenceNumber.CompareTo(
                right.ParentFranchise.GetAllAncestors()
                    .Where(a => a.Entry != null)
                    .First(a => a.Entry!.ParentFranchise.Id == left.ParentFranchise.Id)
                    .Entry!
                    .SequenceNumber);
        }

        return this.Compare(
            new FranchiseListItem(left.ParentFranchise),
            new FranchiseListItem(right.ParentFranchise));
    }

    private int CompareTitle(ListItem left, ListItem right) =>
        this.titleAndOtherComparer.Compare(left, right);

    private int CompareTitle(FranchiseListItem left, ListItem right)
    {
        int result = this.titleComparer.Compare(this.getFranchiseTitle(left.Franchise), this.getTitle(right));
        return result != 0 ? result : this.otherComparer.Compare(left, right);
    }
}
