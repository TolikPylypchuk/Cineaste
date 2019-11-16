using System;
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
            => series.IsWatched
                ? series.Kind.ColorForWatchedSeries
                : series.Status != SeriesStatus.NotStarted
                    ? series.Kind.ColorForNotWatchedSeries
                    : series.Kind.ColorForNotReleasedSeries;

        public static string GetActiveColor(this MovieSeries movieSeries)
        {
            var firstEntry = GetFirstEntry(movieSeries);
            return firstEntry.Movie?.GetActiveColor() ?? firstEntry.Series!.GetActiveColor();
        }

        public static MovieSeriesEntry GetFirstEntry(this MovieSeries movieSeries)
        {
            var firstEntry = movieSeries.Entries.OrderBy(entry => entry.SequenceNumber).First();
            return firstEntry.MovieSeries == null ? firstEntry : firstEntry.MovieSeries.GetFirstEntry();
        }
    }
}
