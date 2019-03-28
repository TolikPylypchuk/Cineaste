using System;
using System.Windows.Media;

using MovieList.Config;
using MovieList.Data.Models;

using static MovieList.Services.Util;

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
                  config != null ? GetColor(movieSeries, config) : Colors.Black)
        {
            this.MovieSeries = movieSeries;
        }

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
            } else if (IsAncestor(this.MovieSeries, other.MovieSeries))
            {
                result = 1;
            } else if (IsAncestor(other.MovieSeries, this.MovieSeries))
            {
                result = -1;
            } else if (this.MovieSeries.RootSeries.Id == other.MovieSeries.RootSeries.Id)
            {
                var (ancestor1, ancestor2) = GetDistinctAncestors(this.MovieSeries, other.MovieSeries);
                result = ancestor1.OrdinalNumber?.CompareTo(ancestor2.OrdinalNumber) ?? 0;
            } else
            {
                result = GetTitleToCompare(this.MovieSeries).CompareTo(GetTitleToCompare(other.MovieSeries));

                if (result == 0)
                {
                    result = GetFirstYear(this.MovieSeries).CompareTo(GetFirstYear(other.MovieSeries));
                }
            }

            return result;
        }

        private int CompareToEntry(ListItem item, MovieSeriesEntry? entry)
            => entry == null
                ? CompareTitleOrYear(item)
                : entry.MovieSeriesId == this.MovieSeries.Id
                    ? -1
                    : this.CompareTo(new MovieSeriesListItem(entry.MovieSeries));

        private int CompareTitleOrYear(ListItem item)
        {
            int result = GetTitleToCompare(this.MovieSeries).CompareTo(item.Title);
            return result != 0 ? result : this.Year.CompareTo(item.Year);
        }
    }
}
