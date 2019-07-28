using System;

using MovieList.Config;
using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public class MovieSeriesListItem : ListItem
    {
        public MovieSeriesListItem(MovieSeries movieSeries)
            : this(movieSeries, null)
        { }

        public MovieSeriesListItem(MovieSeries movieSeries, Configuration? config)
            : base(
                null,
                movieSeries.Title != null ? $"{movieSeries.Title.Name}:" : String.Empty,
                movieSeries.OriginalTitle != null ? $"{movieSeries.OriginalTitle.Name}:" : String.Empty,
                String.Empty,
                movieSeries.GetColor(config))
            => this.MovieSeries = movieSeries;

        public MovieSeries MovieSeries { get; }

        public override int CompareTo(MovieListItem other)
            => this.CompareToEntry(other, other.Movie.Entry);

        public override int CompareTo(SeriesListItem other)
            => this.CompareToEntry(other, other.Series.Entry);

        public override int CompareTo(MovieSeriesListItem other)
        {
            int result;

            if (this.MovieSeries.Id == other.MovieSeries.Id)
            {
                result = 0;
            } else if (this.MovieSeries.IsDescendantOf(other.MovieSeries))
            {
                result = 1;
            } else if (other.MovieSeries.IsDescendantOf(this.MovieSeries))
            {
                result = -1;
            } else if (this.MovieSeries.RootSeries.Id == other.MovieSeries.RootSeries.Id)
            {
                var (ancestor1, ancestor2) = this.MovieSeries.GetDistinctAncestors(other.MovieSeries);
                result = ancestor1.OrdinalNumber?.CompareTo(ancestor2.OrdinalNumber) ?? 0;
            } else
            {
                result = this.MovieSeries.GetTitleName().CompareTo(other.MovieSeries.GetTitleName());

                if (result == 0)
                {
                    result = this.MovieSeries.GetStartYear().CompareTo(other.MovieSeries.GetStartYear());
                }
            }

            return result;
        }

        public override void UpdateColor(Configuration? config)
            => this.Color = this.MovieSeries.GetColor(config);

        private int CompareToEntry(ListItem item, MovieSeriesEntry? entry)
            => entry == null
                ? CompareTitleOrYear(item)
                : entry.MovieSeriesId == this.MovieSeries.Id
                    ? -1
                    : this.CompareTo(new MovieSeriesListItem(entry.MovieSeries));

        private int CompareTitleOrYear(ListItem item)
        {
            int result = this.MovieSeries.GetTitleName().CompareTo(item.Title);
            return result != 0 ? result : this.Year.CompareTo(item.Year);
        }

        public override void OpenSidePanel(SidePanelViewModel sidePanel)
            => sidePanel.OpenMovieSeries.ExecuteIfCan(this.MovieSeries);
    }
}
