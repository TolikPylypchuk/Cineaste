using MovieList.Core.Data.Models;
using MovieList.Data.Models;

namespace MovieList.Core.ListItems
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

        public override FranchiseEntry? Entry
            => this.Movie.Entry;
    }
}
