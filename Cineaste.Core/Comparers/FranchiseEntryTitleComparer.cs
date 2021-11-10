namespace Cineaste.Core.Comparers;

public class FranchiseEntryTitleComparer : NullableComparerBase<FranchiseEntry>
{
    private readonly TitleComparer titleComparer;
    private readonly IComparer<FranchiseEntry> baseComparer;

    public FranchiseEntryTitleComparer(
        CultureInfo culture,
        NullComparison nullComparison = NullComparison.NullsFirst)
        : base(nullComparison)
    {
        this.titleComparer = new TitleComparer(culture);

        this.baseComparer = ComparerBuilder.For<FranchiseEntry>()
            .OrderBy(this.GetTitleName, this.titleComparer)
            .ThenBy(entry => entry.GetStartYear())
            .ThenBy(entry => entry.GetEndYear());
    }

    protected override bool EqualsSafe(FranchiseEntry x, FranchiseEntry y) =>
        this.titleComparer.Equals(this.GetTitleName(x), this.GetTitleName(y)) &&
                x.GetStartYear() == y.GetStartYear() &&
                x.GetEndYear() == y.GetEndYear();

    protected override int GetHashCodeSafe(FranchiseEntry x) =>
        HashCode.Combine(this.GetTitleName(x), x.GetStartYear(), x.GetEndYear());

    protected override int CompareSafe(FranchiseEntry x, FranchiseEntry y) =>
        this.baseComparer.Compare(x, y);

    private string GetTitleName(FranchiseEntry entry) =>
        entry.GetTitle()?.Name ?? String.Empty;
}
