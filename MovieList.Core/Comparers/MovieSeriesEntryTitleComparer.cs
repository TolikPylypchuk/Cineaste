using System;
using System.Collections.Generic;
using System.Globalization;

using MovieList.Data.Models;

namespace MovieList.Comparers
{
    public class MovieSeriesEntryTitleComparer : IComparer<MovieSeriesEntry>
    {
        private readonly TitleComparer titleComparer;

        public MovieSeriesEntryTitleComparer(CultureInfo culture)
            => this.titleComparer = new TitleComparer(culture);

        public int Compare(MovieSeriesEntry left, MovieSeriesEntry right)
        {
            int result = this.titleComparer.Compare(this.GetTitleName(left), this.GetTitleName(right));

            if (result != 0)
            {
                return result;
            }

            result = left.GetStartYear().CompareTo(right.GetStartYear());

            return result != 0 ? result : left.GetEndYear().CompareTo(right.GetEndYear());
        }

        private string GetTitleName(MovieSeriesEntry entry)
            => entry.GetTitle()?.Name ?? String.Empty;
    }
}
