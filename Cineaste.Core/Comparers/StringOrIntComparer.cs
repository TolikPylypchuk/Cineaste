namespace Cineaste.Core.Comparers;

public class StringOrIntComparer : NullableComparerBase<object>
{
    private readonly IComparer<string> stringComparer;
    private readonly IEqualityComparer<string> stringEqualityComparer;

    public StringOrIntComparer(
        IComparer<string> stringComparer,
        IEqualityComparer<string> stringEqualityComparer,
        NullComparison nullComparison = NullComparison.NullsFirst)
        : base(nullComparison)
    {
        this.stringComparer = stringComparer;
        this.stringEqualityComparer = stringEqualityComparer;
    }

    protected override bool EqualsSafe(object x, object y) =>
        (x, y) switch
        {
            (string leftStr, string rightStr) => this.stringEqualityComparer.Equals(leftStr, rightStr),
            (int leftInt, int rightInt) => leftInt == rightInt,
            _ => false
        };

    protected override int GetHashCodeSafe(object x) =>
        x switch
        {
            string str => this.stringEqualityComparer.GetHashCode(str),
            _ => x.GetHashCode()
        };

    protected override int CompareSafe(object x, object y) =>
        (x, y) switch
        {
            (string leftStr, string rightStr) => this.stringComparer.Compare(leftStr, rightStr),
            (int leftInt, int rightInt) => leftInt.CompareTo(rightInt),
            _ => throw new NotSupportedException($"{nameof(StringOrIntComparer)} only compares strings and ints")
        };
}
