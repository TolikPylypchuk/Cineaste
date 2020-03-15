using MovieList.Data.Models;

namespace MovieList.ListItems
{
    public class MovieListItem : ListItem
    {
        public MovieListItem(Movie movie)
            : base(
                $"M-{movie.Id}",
                movie.Entry,
                movie.Title.Name,
                movie.OriginalTitle.Name,
                movie.Year.ToString(),
                movie.Year,
                movie.Year,
                movie.GetActiveColor())
            => this.Movie = movie;

        public Movie Movie { get; }

        public override MovieSeriesEntry? Entry
            => this.Movie.Entry;
    }
}
