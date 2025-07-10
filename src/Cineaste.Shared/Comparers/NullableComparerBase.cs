namespace Cineaste.Shared.Comparers;

public enum NullComparison { NullsFirst, NullsLast }

public abstract class NullableComparerBase<T> : Comparer<T>, IEqualityComparer<T>
{
    protected NullableComparerBase(NullComparison nullComparison) =>
        this.NullComparison = nullComparison;

    public NullComparison NullComparison { get; }

    public bool Equals([AllowNull] T x, [AllowNull] T y) =>
        (x, y) switch
        {
            (null, null) => true,
            (_, null) or (null, _) => false,
            var (left, right) => this.EqualsSafe(left, right)
        };

    public int GetHashCode([AllowNull] T x) =>
        x is null ? 0 : this.GetHashCodeSafe(x);

    public override int Compare([AllowNull] T x, [AllowNull] T y) =>
        (x, y) switch
        {
            (null, null) => 0,
            (_, null) => this.NullComparison == NullComparison.NullsFirst ? 1 : -1,
            (null, _) => this.NullComparison == NullComparison.NullsFirst ? -1 : 1,
            var (left, right) => this.CompareSafe(left, right)
        };

    protected abstract bool EqualsSafe([DisallowNull] T x, [DisallowNull] T y);

    protected abstract int GetHashCodeSafe([DisallowNull] T x);

    protected abstract int CompareSafe([DisallowNull] T x, [DisallowNull] T y);
}
