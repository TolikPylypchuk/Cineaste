using System;
using System.Globalization;

using MovieList.Core.Data.Models;
using MovieList.Data.Models;

namespace MovieList.Core.Comparers
{
    public class FranchiseEntryTitleComparer : NullableComparerBase<FranchiseEntry>
    {
        private readonly TitleComparer titleComparer;

        public FranchiseEntryTitleComparer(
            CultureInfo culture,
            NullComparison nullComparison = NullComparison.NullsFirst)
            : base(nullComparison)
            => this.titleComparer = new TitleComparer(culture);

        protected override int CompareSafe(FranchiseEntry left, FranchiseEntry right)
        {
            int result = this.titleComparer.Compare(this.GetTitleName(left), this.GetTitleName(right));

            if (result != 0)
            {
                return result;
            }

            result = left.GetStartYear().CompareTo(right.GetStartYear());

            return result != 0 ? result : left.GetEndYear().CompareTo(right.GetEndYear());
        }

        private string GetTitleName(FranchiseEntry entry)
            => entry.GetTitle()?.Name ?? String.Empty;
    }
}
