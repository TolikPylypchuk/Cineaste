using System.Windows.Media;

using MovieList.Data.Models;

namespace MovieList.ViewModels.ListItems
{
    public class MovieListItem : ListItem
    {
        public MovieListItem(Movie movie)
            : base(
                  movie.Entry,
                  movie.Title.Name,
                  movie.OriginalTitle.Name,
                  movie.Year.ToString(),
                  (Color)ColorConverter.ConvertFromString(movie.Kind.ColorForMovie))
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
