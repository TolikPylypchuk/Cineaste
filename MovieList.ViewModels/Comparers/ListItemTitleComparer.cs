using System;
using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;
using MovieList.ListItems;

namespace MovieList.Comparers
{
    public sealed class ListItemTitleComparer : IComparer<ListItem>
    {
        private ListItemTitleComparer()
        { }

        public static ListItemTitleComparer Instance { get; } = new ListItemTitleComparer();

        public int Compare(ListItem x, ListItem y)
            => (x, y) switch
            {
                (MovieListItem left, MovieListItem right) => this.Compare(left, right),
                (MovieListItem left, SeriesListItem right) => this.Compare(left, right),
                (MovieListItem left, MovieSeriesListItem right) => this.Compare(left, right),

                (SeriesListItem left, MovieListItem right) => this.Compare(left, right),
                (SeriesListItem left, SeriesListItem right) => this.Compare(left, right),
                (SeriesListItem left, MovieSeriesListItem right) => this.Compare(left, right),

                (MovieSeriesListItem left, MovieListItem right) => this.Compare(left, right),
                (MovieSeriesListItem left, SeriesListItem right) => this.Compare(left, right),
                (MovieSeriesListItem left, MovieSeriesListItem right) => this.Compare(left, right),

                _ => throw new NotSupportedException(
                    $"Types of list items to compare are not supported: {x.GetType()}, {y.GetType()}")
            };

        private int Compare(MovieListItem left, MovieListItem right)
            => left.Movie.Id == right.Movie.Id
                ? 0
                : this.CompareEntries(left, right, left.Movie.Entry, right.Movie.Entry);

        private int Compare(MovieListItem left, SeriesListItem right)
            => this.CompareEntries(left, right, left.Movie.Entry, right.Series.Entry);

        private int Compare(MovieListItem left, MovieSeriesListItem right)
            => this.Compare(right, left) * -1;

        private int Compare(SeriesListItem left, MovieListItem right)
            => this.CompareEntries(left, right, left.Series.Entry, right.Movie.Entry);

        private int Compare(SeriesListItem left, SeriesListItem right)
            => left.Series.Id == right.Series.Id
                ? 0
                : this.CompareEntries(left, right, left.Series.Entry, right.Series.Entry);

        private int Compare(SeriesListItem left, MovieSeriesListItem right)
            => this.Compare(right, left) * -1;

        private int Compare(MovieSeriesListItem left, MovieListItem right)
            => this.CompareEntries(left, right, right.Movie.Entry);

        private int Compare(MovieSeriesListItem left, SeriesListItem right)
            => this.CompareEntries(left, right, right.Series.Entry);

        private int Compare(MovieSeriesListItem left, MovieSeriesListItem right)
        {
            int result;

            if (left.MovieSeries.Id == right.MovieSeries.Id)
            {
                result = 0;
            } else if (left.MovieSeries.IsDescendantOf(right.MovieSeries))
            {
                result = 1;
            } else if (right.MovieSeries.IsDescendantOf(left.MovieSeries))
            {
                result = -1;
            } else if (left.MovieSeries.GetRootSeries().Id == right.MovieSeries.GetRootSeries().Id)
            {
                var (ancestor1, ancestor2) = left.MovieSeries.GetDistinctAncestors(right.MovieSeries);
                result = ancestor1.Entry?.SequenceNumber.CompareTo(ancestor2.Entry?.SequenceNumber) ?? 0;
            } else
            {
                result = TitleComparer.Instance.Compare(
                    left.MovieSeries.GetTitle()?.Name ?? String.Empty,
                    right.MovieSeries.GetTitle()?.Name ?? String.Empty);

                if (result == 0)
                {
                    result = left.MovieSeries.GetStartYear().CompareTo(right.MovieSeries.GetStartYear());

                    if (result == 0)
                    {
                        result = left.MovieSeries.GetEndYear().CompareTo(right.MovieSeries.GetEndYear());
                    }
                }
            }

            return result;
        }

#pragma warning disable 8509

        private int CompareEntries(
            ListItem left,
            ListItem right,
            MovieSeriesEntry? leftEntry,
            MovieSeriesEntry? rightEntry)
            => (leftEntry, rightEntry) switch
            {
                (null, null) => this.CompareTitleOrYear(left, right),
                (var entry, null) => this.Compare(new MovieSeriesListItem(entry.ParentSeries), right),
                (null, var entry) => this.Compare(left, new MovieSeriesListItem(entry.ParentSeries)),
                var (entry1, entry2) => entry1.ParentSeriesId == entry2.ParentSeriesId
                    ? entry1.SequenceNumber.CompareTo(entry2.SequenceNumber)
                    : this.CompareEntries(entry1, entry2)
            };

#pragma warning restore 8509

        private int CompareEntries(MovieSeriesListItem left, ListItem right, MovieSeriesEntry? entry)
        {
            if (entry == null)
            {
                return this.CompareTitleOrYear(left, right);
            }

            if (left.MovieSeries.IsDescendantOf(entry.ParentSeries))
            {
                return -1;
            }

            return this.Compare(left, new MovieSeriesListItem(entry.ParentSeries));
        }

        private int CompareEntries(MovieSeriesEntry entry1, MovieSeriesEntry entry2)
        {
            if (entry1.ParentSeries.IsDescendantOf(entry2.ParentSeries))
            {
                return entry1.ParentSeries.GetAllAncestors()
                        .Where(a => a.Entry != null)
                        .First(a => a.Entry!.ParentSeriesId == entry2.ParentSeriesId)
                        .Entry!
                    .SequenceNumber
                    .CompareTo(entry2.SequenceNumber);
            }

            if (entry2.ParentSeries.IsDescendantOf(entry1.ParentSeries))
            {
                return entry1.SequenceNumber.CompareTo(
                    entry2.ParentSeries.GetAllAncestors()
                        .Where(a => a.Entry != null)
                        .First(a => a.Entry!.ParentSeriesId == entry1.ParentSeriesId)
                        .Entry!
                        .SequenceNumber);
            }

            return this.Compare(
                new MovieSeriesListItem(entry1.ParentSeries),
                new MovieSeriesListItem(entry2.ParentSeries));
        }

        private int CompareTitleOrYear(ListItem left, ListItem right)
        {
            int result = TitleComparer.Instance.Compare(left.Title, right.Title);
            return result != 0 ? result : left.Year.CompareTo(right.Year);
        }

        private int CompareTitleOrYear(MovieSeriesListItem left, ListItem right)
        {
            int result = TitleComparer.Instance.Compare(
                left.MovieSeries.GetTitle()?.Name ?? String.Empty, right.Title);

            return result != 0 ? result : left.Year.CompareTo(right.Year);
        }
    }
}
