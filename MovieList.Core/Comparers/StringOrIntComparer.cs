using System;
using System.Collections.Generic;

namespace MovieList.Comparers
{
    public class StringOrIntComparer : IComparer<object>
    {
        private readonly IComparer<string> stringComparer;

        public StringOrIntComparer(IComparer<string> stringComparer)
            => this.stringComparer = stringComparer;

        public int Compare(object x, object y)
            => (x, y) switch
            {
                (string leftStr, string rightStr) => this.stringComparer.Compare(leftStr, rightStr),
                (int leftInt, int rightInt) => leftInt.CompareTo(rightInt),
                _ => throw new NotSupportedException($"{nameof(StringOrIntComparer)} only compares strings and ints")
            };
    }
}
