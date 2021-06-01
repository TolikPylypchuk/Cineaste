using System;
using System.Collections.Generic;
using System.Globalization;

using MovieList.Core.Data.Models;
using MovieList.Data.Models;

using Nito.Comparers;

namespace MovieList.Core.Comparers
{
    public class FranchiseEntryTitleComparer : NullableComparerBase<FranchiseEntry>
    {
        private readonly TitleComparer titleComparer;
        private readonly IComparer<FranchiseEntry> baseComparer;

        public FranchiseEntryTitleComparer(
            CultureInfo culture,
            NullComparison nullComparison = NullComparison.NullsFirst)
            : base(nullComparison)
        {
            this.titleComparer = new TitleComparer(culture);

            this.baseComparer = ComparerBuilder.For<FranchiseEntry>()
                .OrderBy(this.GetTitleName)
                .ThenBy(entry => entry.GetStartYear())
                .ThenBy(entry => entry.GetEndYear());
        }

        protected override bool EqualsSafe(FranchiseEntry left, FranchiseEntry right) =>
            this.titleComparer.Equals(this.GetTitleName(left), this.GetTitleName(right)) &&
                    left.GetStartYear() == right.GetStartYear() &&
                    left.GetEndYear() == right.GetEndYear();

        protected override int GetHashCodeSafe(FranchiseEntry entry) =>
            HashCode.Combine(this.GetTitleName(entry), entry.GetStartYear(), entry.GetEndYear());

        protected override int CompareSafe(FranchiseEntry left, FranchiseEntry right) =>
            this.baseComparer.Compare(left, right);

        private string GetTitleName(FranchiseEntry entry) =>
            entry.GetTitle()?.Name ?? String.Empty;
    }
}
