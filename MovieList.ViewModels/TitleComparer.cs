using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MovieList
{
    public class TitleComparer : IComparer<string>
    {
        private TitleComparer() { }

        public static TitleComparer Instance { get; } = new TitleComparer();

        public int Compare([AllowNull] string x, [AllowNull] string y)
            => Comparer<string>.Default.Compare(this.Normalize(x), this.Normalize(y));

        private string? Normalize(string? title)
            => title?.ToLower()
                .Replace(":", "")
                .Replace(" - ", " ");
    }
}
