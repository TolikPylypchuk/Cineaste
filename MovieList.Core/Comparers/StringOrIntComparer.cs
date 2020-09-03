using System;
using System.Collections.Generic;

namespace MovieList.Core.Comparers
{
    public class StringOrIntComparer : NullableComparerBase<object>
    {
        private readonly IComparer<string> stringComparer;

        public StringOrIntComparer(
            IComparer<string> stringComparer,
            NullComparison nullComparison = NullComparison.NullsFirst)
            : base(nullComparison)
            => this.stringComparer = stringComparer;

        protected override int CompareSafe(object x, object y)
            => (x, y) switch
            {
                (string leftStr, string rightStr) => this.stringComparer.Compare(leftStr, rightStr),
                (int leftInt, int rightInt) => leftInt.CompareTo(rightInt),
                _ => throw new NotSupportedException($"{nameof(StringOrIntComparer)} only compares strings and ints")
            };
    }
}
