namespace Cineaste.Core;

internal sealed class StringOrIntComparer(IComparer<string> stringComparer) : IComparer<IEnumerable<object>>
{
    public int Compare(IEnumerable<object>? x, IEnumerable<object>? y)
    {
        if (x is null)
        {
            return y is null ? 0 : -1;
        }

        if (y is null)
        {
            return 1;
        }

        using var xEnumerator = x.GetEnumerator();
        using var yEnumerator = y.GetEnumerator();

        while (true)
        {
            if (!xEnumerator.MoveNext())
            {
                return !yEnumerator.MoveNext() ? 0 : -1;
            }

            if (!yEnumerator.MoveNext())
            {
                return 1;
            }

            var result = this.CompareSingle(xEnumerator.Current, yEnumerator.Current);

            if (result != 0)
            {
                return result;
            }
        }
    }

    private int CompareSingle(object x, object y) =>
        (x, y) switch
        {
            (string leftStr, string rightStr) => stringComparer.Compare(leftStr, rightStr),
            (int leftInt, int rightInt) => leftInt.CompareTo(rightInt),
            (int leftInt, string rightStr) => stringComparer.Compare(leftInt.ToString(), rightStr),
            (string leftStr, int rightInt) => stringComparer.Compare(leftStr, rightInt.ToString()),
            _ => throw new NotSupportedException($"{nameof(StringOrIntComparer)} only compares strings and ints")
        };
}
