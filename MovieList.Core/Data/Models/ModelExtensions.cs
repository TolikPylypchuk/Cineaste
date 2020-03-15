using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using static MovieList.Data.Constants;

namespace MovieList.Data.Models
{
    public static class ModelExtensions
    {
        public static string GetNumberToDisplay(this MovieSeriesEntry? entry)
            => entry != null
                ? entry.ParentSeries.IsLooselyConnected
                    ? $"({entry.DisplayNumber})"
                    : entry.DisplayNumber?.ToString() ?? NoDisplayNumberPlaceholder
                : String.Empty;

        public static string AsDisplayNumber(this int? number, bool inParentheses)
            => number != null
                ? inParentheses ? $"({number})" : number.ToString() ?? NoDisplayNumberPlaceholder
                : NoDisplayNumberPlaceholder;

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
            var firstEntry = movieSeries.GetFirstEntry();
            return firstEntry != null
                ? firstEntry.Movie?.GetActiveColor() ?? firstEntry.Series!.GetActiveColor()
                : String.Empty;
        }

        public static Title? GetTitle(this MovieSeries movieSeries)
            => movieSeries.Titles.Count > 0 ? movieSeries.Title! : movieSeries.GetFirstEntry()?.GetTitle();

        public static Title? GetOriginalTitle(this MovieSeries movieSeries)
            => movieSeries.Titles.Count > 0
                ? movieSeries.OriginalTitle!
                : movieSeries.GetFirstEntry()?.GetOriginalTitle();

        public static Title? GetListTitle(this MovieSeries movieSeries)
            => movieSeries.ShowTitles ? movieSeries.Title! : movieSeries.GetFirstEntry()?.GetTitle();

        public static Title? GetTitle(this MovieSeriesEntry entry)
            => entry.Movie?.Title ?? entry.Series?.Title ?? entry.MovieSeries!.GetTitle();

        public static Title? GetOriginalTitle(this MovieSeriesEntry entry)
            => entry.Movie?.OriginalTitle ?? entry.Series?.OriginalTitle ?? entry.MovieSeries!.GetOriginalTitle();

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public static MovieSeriesEntry? GetFirstEntry(this MovieSeries movieSeries)
        {
            var firstEntry = movieSeries.Entries.OrderBy(entry => entry.SequenceNumber).FirstOrDefault();
            return firstEntry != null
                ? firstEntry.MovieSeries == null ? firstEntry : firstEntry.MovieSeries.GetFirstEntry()
                : null;
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public static MovieSeriesEntry? GetLastEntry(this MovieSeries movieSeries)
        {
            var lastEntry = movieSeries.Entries.OrderByDescending(entry => entry.SequenceNumber).FirstOrDefault();
            return lastEntry != null
                ? lastEntry.MovieSeries == null ? lastEntry : lastEntry.MovieSeries.GetLastEntry()
                : null;
        }

        public static MovieSeries GetRootSeries(this MovieSeries movieSeries)
            => movieSeries.Entry == null ? movieSeries : movieSeries.Entry.ParentSeries.GetRootSeries();

        public static bool IsDescendantOf(this MovieSeries? movieSeries, MovieSeries potentialAncestor)
            => movieSeries != null &&
                (movieSeries.Id == potentialAncestor.Id ||
                    (movieSeries.Entry?.ParentSeries.IsDescendantOf(potentialAncestor) ?? false));

        public static bool IsStrictDescendantOf(this MovieSeries? movieSeries, MovieSeries potentialAncestor)
            => movieSeries != potentialAncestor && movieSeries.IsDescendantOf(potentialAncestor);

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
            => movieSeries.GetFirstEntry()?.GetStartYear() ?? default;

        public static int GetStartYear(this MovieSeriesEntry entry)
            => entry.Movie?.Year ?? entry.Series?.StartYear ?? entry.MovieSeries!.GetStartYear();

        public static int GetEndYear(this MovieSeries movieSeries)
            => movieSeries.GetLastEntry()?.GetEndYear() ?? default;

        public static int GetEndYear(this MovieSeriesEntry entry)
            => entry.Movie?.Year ?? entry.Series?.EndYear ?? entry.MovieSeries!.GetEndYear();

        public static string GetYears(this MovieSeriesEntry entry)
            => entry.Movie != null
                ? entry.Movie.Year.ToString()
                : entry.Series != null
                    ? entry.Series.GetYears()
                    : entry.MovieSeries!.GetYears();
        
        public static string GetYears(this Series series)
        {
            int startYear = series.StartYear;
            int endYear = series.EndYear;
            return startYear == endYear ? startYear.ToString() : $"{startYear}-{endYear}";
        }

        public static string GetYears(this MovieSeries movieSeries)
        {
            int startYear = movieSeries.GetStartYear();
            int endYear = movieSeries.GetEndYear();
            return startYear == default
                ? "-"
                : startYear == endYear ? startYear.ToString() : $"{startYear}-{endYear}";
        }
    }
}
