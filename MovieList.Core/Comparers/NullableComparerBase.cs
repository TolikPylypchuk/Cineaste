using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MovieList.Comparers
{
    public enum NullComparison { NullsFirst, NullsLast }

    public abstract class NullableComparerBase<T> : Comparer<T>
    {
        protected NullableComparerBase(NullComparison nullComparison)
            => this.NullComparison = nullComparison;

        public NullComparison NullComparison { get; }

        public override int Compare([AllowNull] T x, [AllowNull] T y)
            => (x, y) switch
            {
                (null, null) => 0,
                (_, null) => this.NullComparison == NullComparison.NullsFirst ? 1 : -1,
                (null, _) => this.NullComparison == NullComparison.NullsFirst ? -1 : 1,
                var (left, right) => this.CompareSafe(left, right)
            };

        protected abstract int CompareSafe([DisallowNull] T x, [DisallowNull] T y);
    }
}
