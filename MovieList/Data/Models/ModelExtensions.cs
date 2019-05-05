using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using MovieList.Config;

namespace MovieList.Data.Models
{
    public static class ModelExtensions
    {
        public static List<MovieSeries> GetAllAncestors(this MovieSeries? series)
        {
            if (series == null)
            {
                return new List<MovieSeries>();
            }

            var result = new List<MovieSeries>();
            result.AddRange(series.ParentSeries.GetAllAncestors());
            result.Add(series);

            return result;
        }

        public static (MovieSeries, MovieSeries) GetDistinctAncestors(this MovieSeries series1, MovieSeries series2)
            => series1.GetAllAncestors()
                .Zip(series2.GetAllAncestors(), (a, b) => (Series1: a, Series2: b))
                .First(ancestors => ancestors.Series1.Id != ancestors.Series2.Id);

        public static bool IsDescendantOf(this MovieSeries? series, MovieSeries potentialAncestor)
            => series != null &&
                (series.Id == potentialAncestor.Id || series.ParentSeries.IsDescendantOf(potentialAncestor));

        public static MovieSeriesEntry GetFirstEntry(this MovieSeries movieSeries)
            => movieSeries.Entries.Count != 0
                ? movieSeries.Entries.OrderBy(entry => entry.OrdinalNumber).First()
                : movieSeries.Parts.OrderBy(part => part.OrdinalNumber).First().GetFirstEntry();

        public static string GetTitleToCompare(this MovieSeries movieSeries)
        {
            if (movieSeries.Title != null)
            {
                return movieSeries.Title.Name;
            }

            var firstEntry = movieSeries.GetFirstEntry();
            var title = firstEntry.Movie != null ? firstEntry.Movie.Title : firstEntry.Series!.Title;

            return title.Name;
        }

        public static int GetFirstYear(this MovieSeries movieSeries)
        {
            var firstEntry = movieSeries.GetFirstEntry();
            return firstEntry.Movie != null ? firstEntry.Movie.Year : firstEntry.Series!.StartYear;
        }

        public static string GetDisplayNumber(this MovieSeriesEntry? entry)
            => entry != null
                ? entry.MovieSeries.IsLooselyConnected
                    ? $"({entry.OrdinalNumber})"
                    : entry.DisplayNumber?.ToString() ?? "-"
                : String.Empty;

        public static Color GetColor(this Movie movie, Configuration? config)
            => movie.IsReleased
                ? movie.IsWatched
                    ? (Color)ColorConverter.ConvertFromString(movie.Kind.ColorForMovie)
                    : config?.NotWatchedColor ?? Colors.Red
                : config?.NotReleasedColor ?? Colors.Red;

        public static Color GetColor(this Series series, Configuration? config)
            => series.Seasons.First().IsReleased
                ? series.IsWatched
                    ? (Color)ColorConverter.ConvertFromString(series.Kind.ColorForSeries)
                    : config?.NotWatchedColor ?? Colors.Red
                : config?.NotReleasedColor ?? Colors.Red;

        public static Color GetColor(this MovieSeries movieSeries, Configuration? config)
        {
            var firstEntry = GetFirstEntry(movieSeries);
            return firstEntry.Movie != null ? firstEntry.Movie.GetColor(config) : firstEntry.Series!.GetColor(config);
        }

        public static IEnumerable<Season> GetOrderedSeasons(this Series series)
            => series.Seasons
                .OrderBy(season => season.Periods.OrderBy(period => period.StartYear).ThenBy(period => period.StartMonth).First().StartYear)
                .OrderBy(season => season.Periods.OrderBy(period => period.StartYear).ThenBy(period => period.StartMonth).First().StartMonth);
    }
}
