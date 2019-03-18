using System.Linq;
using System.Windows.Media;

using MovieList.Data.Models;

using static MovieList.Services.Util;

namespace MovieList.ViewModels.ListItems
{
    public class SeriesListItem : ListItemBase
    {
        private SeriesListItem(Series series, MovieSeriesEntry? entry, string title, string originalTitle, string year, Color color)
            : base(entry, title, originalTitle, year, color)
        {
            this.Series = series;
        }

        public Series Series { get; }

        public static SeriesListItem FromSeries(Series series)
        {
            string title = series.Titles
                .Where(title => !title.IsOriginal)
                .OrderByDescending(title => title.Priority)
                .First()
                .Name;

            string originalTitle = series.Titles
                .Where(title => title.IsOriginal)
                .OrderByDescending(title => title.Priority)
                .First()
                .Name;

            string startYear = series.Seasons
                .SelectMany(season => season.Periods)
                .OrderBy(period => period.StartYear)
                .ThenBy(period => period.StartMonth)
                .First()
                .StartYear
                .ToString();

            string endYear = series.Seasons
                .SelectMany(season => season.Periods)
                .OrderByDescending(period => period.EndYear)
                .ThenByDescending(period => period.EndMonth)
                .First()
                .EndYear
                .ToString();

            var color = IntToColor(series.Kind.ColorForSeries);

            return new SeriesListItem(series, series.Entry, title, originalTitle, $"{startYear}-{endYear}", color);
        }
    }
}
