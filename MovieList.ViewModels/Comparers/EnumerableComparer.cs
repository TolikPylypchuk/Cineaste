using System.Collections.Generic;

namespace MovieList.Comparers
{
    public class EnumerableComparer<T> : NullsFirstComparer<IEnumerable<T>>
    {
        public EnumerableComparer()
            : this(Comparer<T>.Default)
        { }

        public EnumerableComparer(IComparer<T> comparer)
            => this.comparer = comparer;

        private readonly IComparer<T> comparer;

        protected override int CompareNonNull(IEnumerable<T> x, IEnumerable<T> y)
        {
            using var leftEnumerator = x.GetEnumerator();
            using var rightEnumerator = y.GetEnumerator();

            int? result = null;

            while (result is null)
            {
                bool leftPresent = leftEnumerator.MoveNext();
                bool rightPresent = rightEnumerator.MoveNext();

                if (!leftPresent && !rightPresent)
                {
                    result = 0;
                } else if (!leftPresent)
                {
                    result = -1;
                } else if (!rightPresent)
                {
                    result = 1;
                } else
                {
                    int itemResult = this.comparer.Compare(leftEnumerator.Current, rightEnumerator.Current);

                    if (itemResult != 0)
                    {
                        result = itemResult;
                    }
                }
            }

            return result.Value;
        }
    }
}
