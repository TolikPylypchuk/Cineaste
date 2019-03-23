using System.Windows.Media;

using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public class SeriesListItem : ListItem
    {
        public SeriesListItem(Series series)
            : base(
                  series.Entry,
                  series.Title.Name,
                  series.OriginalTitle.Name,
                  $"{series.StartYear}-{series.EndYear}",
                  (Color)ColorConverter.ConvertFromString(series.Kind.ColorForSeries))
        {
            this.Series = series;
        }

        public Series Series { get; }

        public override int CompareTo(MovieListItem other)
            => this.CompareToEntry(other, this.Series.Entry, other.Movie.Entry);

        public override int CompareTo(SeriesListItem other)
            => this.Series.Id == other.Series.Id
                ? 0
                : this.CompareToEntry(other, this.Series.Entry, other.Series.Entry);

        public override int CompareTo(MovieSeriesListItem other)
            => other.CompareTo(this) * -1;
    }
}
