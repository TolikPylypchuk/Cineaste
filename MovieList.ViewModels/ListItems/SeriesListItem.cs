using MovieList.Data.Models;

namespace MovieList.ListItems
{
    public class SeriesListItem : ListItem
    {
        public SeriesListItem(Series series)
            : base(
                  $"S-{series.Id}",
                  series.Entry,
                  series.Title.Name,
                  series.OriginalTitle.Name,
                  series.StartYear != series.EndYear
                      ? $"{series.StartYear} - {series.EndYear}"
                      : series.StartYear.ToString(),
                  series.GetActiveColor())
            => this.Series = series;

        public Series Series { get; }

        public override void Refresh()
            => this.SetProperties(
                this.Series.Entry,
                this.Series.Title.Name,
                this.Series.OriginalTitle.Name,
                this.Series.StartYear != this.Series.EndYear
                    ? $"{this.Series.StartYear} - {this.Series.EndYear}"
                    : this.Series.StartYear.ToString(),
                this.Series.GetActiveColor());
    }
}
