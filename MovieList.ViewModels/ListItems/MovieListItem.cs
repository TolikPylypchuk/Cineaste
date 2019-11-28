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
                movie.GetActiveColor())
            => this.Movie = movie;

        public Movie Movie { get; }

        public override void Refresh()
            => this.SetProperties(
                this.Movie.Entry,
                this.Movie.Title.Name,
                this.Movie.OriginalTitle.Name,
                this.Movie.Year.ToString(),
                this.Movie.GetActiveColor());
    }
}
