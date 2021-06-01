using Cineaste.Core.Data.Models;
using Cineaste.Data.Models;

namespace Cineaste.Core.ListItems
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
                movie.GetActiveColor()) =>
            this.Movie = movie;

        public Movie Movie { get; }

        public override FranchiseEntry? Entry =>
            this.Movie.Entry;
    }
}
