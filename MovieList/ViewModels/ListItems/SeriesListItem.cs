using System.Linq;

using MovieList.Data.Models;
using MovieList.Services;

namespace MovieList.ViewModels.ListItems
{
    public class SeriesListItem : ListItem
    {
        internal SeriesListItem(Series series)
            : base(
                  series.Entry,
                  series.Title.Name,
                  series.OriginalTitle.Name,
                  $"{series.StartYear}-{series.EndYear}",
                  Util.IntToColor(series.Kind.ColorForSeries))
        {
            this.Series = series;
        }

        public Series Series { get; }

        public override string SelectTitleToCompare()
        {
            Title result;

            if (this.Series.Entry == null)
            {
                result = this.Series.Title;
            } else
            {
                var seriesTitle = this.Series.Entry.MovieSeries.Title;

                if (seriesTitle != null)
                {
                    result = seriesTitle;
                } else
                {
                    var firstEntry = this.Series.Entry.MovieSeries.Entries.OrderBy(entry => entry.OrdinalNumber).First();
                    result = firstEntry.Movie != null ? firstEntry.Movie.Title : firstEntry.Series!.Title;
                }
            }

            return result.Name;
        }
    }
}
