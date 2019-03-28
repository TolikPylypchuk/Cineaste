using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using MovieList.Config;
using MovieList.Data.Models;

namespace MovieList.Services
{
    public static class Util
    {
        public static int GetHashCode(params object[] properties)
            => properties.Select(p => p.GetHashCode()).Aggregate(17, (acc, hash) => unchecked(acc * 23 + hash));

        public static (MovieSeries, MovieSeries) GetDistinctAncestors(MovieSeries series1, MovieSeries series2)
            => GetAllAncestors(series1)
                .Zip(GetAllAncestors(series2), (a, b) => (Series1: a, Series2: b))
                .First(ancestors => ancestors.Series1.Id != ancestors.Series2.Id);

        public static List<MovieSeries> GetAllAncestors(MovieSeries? series)
        {
            if (series == null)
            {
                return new List<MovieSeries>();
            }

            var result = new List<MovieSeries>();
            result.AddRange(GetAllAncestors(series.ParentSeries));
            result.Add(series);

            return result;
        }

        public static bool IsAncestor(MovieSeries? series, MovieSeries potentialAncestor)
            => series != null &&
                (series.Id == potentialAncestor.Id || IsAncestor(series.ParentSeries, potentialAncestor));

        public static MovieSeriesEntry GetFirstEntry(MovieSeries movieSeries)
            => movieSeries.Entries.Count != 0
                ? movieSeries.Entries.OrderBy(entry => entry.OrdinalNumber).First()
                : GetFirstEntry(movieSeries.Parts.OrderBy(part => part.OrdinalNumber).First());

        public static string GetTitleToCompare(MovieSeries movieSeries)
        {
            if (movieSeries.Title != null)
            {
                return movieSeries.Title.Name;
            }

            var firstEntry = GetFirstEntry(movieSeries);
            var title = firstEntry.Movie != null ? firstEntry.Movie.Title : firstEntry.Series!.Title;

            return title.Name;
        }

        public static int GetFirstYear(MovieSeries movieSeries)
        {
            var firstEntry = GetFirstEntry(movieSeries);
            return firstEntry.Movie != null ? firstEntry.Movie.Year : firstEntry.Series!.StartYear;
        }

        public static Color GetColor(MovieSeries movieSeries, Configuration uiConfig)
        {
            var firstEntry = GetFirstEntry(movieSeries);
            return firstEntry.Movie != null ? GetColor(firstEntry.Movie, uiConfig) : GetColor(firstEntry.Series!, uiConfig);
        }

        public static Color GetColor(Movie movie, Configuration uiConfig)
            => movie.IsReleased
                ? movie.IsWatched
                    ? (Color)ColorConverter.ConvertFromString(movie.Kind.ColorForMovie)
                    : uiConfig.NotWatchedColor
                : uiConfig.NotReleasedColor;

        public static Color GetColor(Series series, Configuration uiConfig)
            => series.Seasons.First().IsReleased
                ? series.IsWatched
                    ? (Color)ColorConverter.ConvertFromString(series.Kind.ColorForSeries)
                    : uiConfig.NotWatchedColor
                : uiConfig.NotReleasedColor;
    }
}
