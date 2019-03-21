using System.Linq;

using MovieList.Data.Models;
using MovieList.Services;

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
                  Util.IntToColor(movie.Kind.ColorForMovie))
        {
            this.Movie = movie;
        }

        public Movie Movie { get; }

        public override string SelectTitleToCompare()
        {
            Title result;

            if (this.Movie.Entry == null)
            {
                result = this.Movie.Title;
            } else
            {
                var seriesTitle = this.Movie.Entry.MovieSeries.Title;

                if (seriesTitle != null)
                {
                    result = seriesTitle;
                } else
                {
                    var firstEntry = this.Movie.Entry.MovieSeries.Entries.OrderBy(entry => entry.OrdinalNumber).First();
                    result = firstEntry.Movie != null ? firstEntry.Movie.Title : firstEntry.Series!.Title;
                }
            }

            return result.Name;
        }
    }
}
