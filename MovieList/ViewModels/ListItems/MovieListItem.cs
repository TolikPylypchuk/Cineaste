using System.Windows.Media;

using MovieList.Config;
using MovieList.Data.Models;
using MovieList.Services;

namespace MovieList.ViewModels.ListItems
{
    public class MovieListItem : ListItem
    {
        public MovieListItem(Movie movie)
            : this(movie, null)
        { }

        public MovieListItem(Movie movie, Configuration? config)
            : base(
                  movie.Entry,
                  movie.Title.Name,
                  movie.OriginalTitle.Name,
                  movie.Year.ToString(),
                  config != null ? Util.GetColor(movie, config) : Colors.Black)
        {
            this.Movie = movie;
        }

        public Movie Movie { get; }

        public override int CompareTo(MovieListItem other)
            => this.Movie.Id == other.Movie.Id
                ? 0
                : this.CompareToEntry(other, this.Movie.Entry, other.Movie.Entry);

        public override int CompareTo(SeriesListItem other)
            => this.CompareToEntry(other, this.Movie.Entry, other.Series.Entry);

        public override int CompareTo(MovieSeriesListItem other)
            => other.CompareTo(this) * -1;
    }
}
