using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieList.Data.Models
{
    public static class ModelExtensions
    {
        public static string GetDisplayNumber(this MovieSeriesEntry? entry)
            => entry != null
                ? entry.ParentSeries.IsLooselyConnected
                    ? $"({entry.SequenceNumber})"
                    : entry.DisplayNumber?.ToString() ?? "-"
                : String.Empty;

        public static string GetActiveColor(this Movie movie)
            => movie.IsWatched
                ? movie.Kind.ColorForWatchedMovie
                : movie.IsReleased
                    ? movie.Kind.ColorForNotWatchedMovie
                    : movie.Kind.ColorForNotReleasedMovie;

        public static string GetActiveColor(this Series series)
            => series.WatchStatus != SeriesWatchStatus.NotWatched
                ? series.Kind.ColorForWatchedSeries
                : series.ReleaseStatus != SeriesReleaseStatus.NotStarted
                    ? series.Kind.ColorForNotWatchedSeries
                    : series.Kind.ColorForNotReleasedSeries;

        public static string GetActiveColor(this MovieSeries movieSeries)
        {
            var firstEntry = GetFirstEntry(movieSeries);
            return firstEntry.Movie?.GetActiveColor() ?? firstEntry.Series!.GetActiveColor();
        }

        public static Title GetTitle(this MovieSeries movieSeries)
            => movieSeries.ShowTitles ? movieSeries.Title! : movieSeries.GetFirstEntry().GetTitle();

        public static Title GetTitle(this MovieSeriesEntry entry)
            => entry.Movie?.Title ?? entry.Series?.Title ?? entry.MovieSeries!.GetTitle();

        public static MovieSeriesEntry GetFirstEntry(this MovieSeries movieSeries)
        {
            var firstEntry = movieSeries.Entries.OrderBy(entry => entry.SequenceNumber).First();
            return firstEntry.MovieSeries == null ? firstEntry : firstEntry.MovieSeries.GetFirstEntry();
        }

        public static MovieSeriesEntry GetLastEntry(this MovieSeries movieSeries)
        {
            var lastEntry = movieSeries.Entries.OrderByDescending(entry => entry.SequenceNumber).First();
            return lastEntry.MovieSeries == null ? lastEntry : lastEntry.MovieSeries.GetLastEntry();
        }

        public static MovieSeries GetRootSeries(this MovieSeries movieSeries)
            => movieSeries.Entry == null ? movieSeries : movieSeries.Entry.ParentSeries.GetRootSeries();

        public static bool IsDescendantOf(this MovieSeries? series, MovieSeries potentialAncestor)
            => series != null &&
                (series.Id == potentialAncestor.Id ||
                    (series.Entry?.ParentSeries.IsDescendantOf(potentialAncestor) ?? false));

        public static IEnumerable<MovieSeries> GetAllAncestors(this MovieSeries? series)
        {
            if (series == null)
            {
                yield break;
            }

            if (series.Entry != null)
            {
                foreach (var ancestor in series.Entry.ParentSeries.GetAllAncestors())
                {
                    yield return ancestor;
                }
            }

            yield return series;
        }

        public static (MovieSeries, MovieSeries) GetDistinctAncestors(this MovieSeries series1, MovieSeries series2)
            => series1.GetAllAncestors()
                .Zip(series2.GetAllAncestors(), (a, b) => (Series1: a, Series2: b))
                .First(ancestors => ancestors.Series1.Id != ancestors.Series2.Id);

        public static int GetStartYear(this MovieSeries movieSeries)
        {
            var firstEntry = movieSeries.GetFirstEntry();
            return firstEntry.Movie?.Year ?? firstEntry.Series?.StartYear ?? firstEntry.MovieSeries!.GetStartYear();
        }

        public static int GetEndYear(this MovieSeries movieSeries)
        {
            var lastEntry = movieSeries.GetLastEntry();
            return lastEntry.Movie?.Year ?? lastEntry.Series?.EndYear ?? lastEntry.MovieSeries!.GetEndYear();
        }
    }
}
