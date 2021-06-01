using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using Nito.Comparers;

namespace MovieList.Core.Comparers
{
    public sealed class TitleComparer : NullableComparerBase<string>
    {
        private const string Space = " ";

        private static readonly Regex NumberRegex = new("([0-9]+)", RegexOptions.Compiled);
        private static readonly Regex SpacesRegex = new("\\s+", RegexOptions.Compiled);

        private readonly StringComparer stringComparer;
        private readonly IComparer<IEnumerable<object>> comparer;

        public TitleComparer(CultureInfo culture, NullComparison nullComparison = NullComparison.NullsFirst)
            : base(nullComparison)
        {
            this.stringComparer = culture.CompareInfo.GetStringComparer(CompareOptions.None);
            this.comparer = new StringOrIntComparer(this.stringComparer, this.stringComparer).Sequence();
        }

        protected override bool EqualsSafe(string x, string y) =>
            this.stringComparer.Equals(x, y);

        protected override int GetHashCodeSafe(string x) =>
            this.stringComparer.GetHashCode(x);

        protected override int CompareSafe(string x, string y) =>
            this.comparer.Compare(this.NormalizeAndConvert(x), this.NormalizeAndConvert(y));

        private IEnumerable<object> NormalizeAndConvert(string title) =>
            NumberRegex.Split(this.Normalize(title)).Select(this.Convert);

        private string Normalize(string title) =>
            SpacesRegex.Replace(
                title.ToLower()
                    .Replace(":", Space)
                    .Replace(";", Space)
                    .Replace("!", Space)
                    .Replace("?", Space)
                    .Replace(".", Space)
                    .Replace(",", Space)
                    .Replace(" - ", Space)
                    .Replace(" – ", Space)
                    .Replace("-", Space)
                    .Replace("'", Space)
                    .Replace("’", Space)
                    .Replace("\"", Space),
                Space);

        private object Convert(string title) =>
            Int32.TryParse(title, out int result) ? (object)result : title;
    }
}
