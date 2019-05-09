using MovieList.Config;
using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public class SeriesListItem : ListItem
    {
        public SeriesListItem(Series series)
            : this(series, null)
        { }

        public SeriesListItem(Series series, Configuration? config)
            : base(
                series.Entry,
                series.Title.Name,
                series.OriginalTitle.Name,
                series.StartYear != series.EndYear
                    ? $"{series.StartYear}-{series.EndYear}"
                    : series.StartYear.ToString(),
                series.GetColor(config))
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

        public override void UpdateColor(Configuration? config)
            => this.Color = this.Series.GetColor(config);

        public override void OpenSidePanel(SidePanelViewModel sidePanelViewModel)
        {
            if (sidePanelViewModel.OpenSeries.CanExecute(this.Series))
            {
                sidePanelViewModel.OpenSeries.Execute(this.Series);
            }
        }
    }
}
