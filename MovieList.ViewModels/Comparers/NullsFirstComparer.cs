using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MovieList.Comparers
{
    public abstract class NullsFirstComparer<T> : IComparer<T>
    {
#pragma warning disable 8509

        public int Compare([AllowNull] T x, [AllowNull] T y)
            => (x, y) switch
            {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                var (left, right) => this.CompareNonNull(left, right)
            };

#pragma warning restore 8509

        protected abstract int CompareNonNull(T x, T y);
    }
}
