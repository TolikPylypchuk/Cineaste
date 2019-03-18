using System.Linq;
using System.Windows.Media;

using MovieList.Data.Models;

using static MovieList.Services.Util;

namespace MovieList.ViewModels.ListItems
{
    public class MovieListItem : ListItemBase
    {
        private MovieListItem(Movie movie, MovieSeriesEntry? entry, string title, string originalTitle, string year, Color color)
            : base(entry, title, originalTitle, year, color)
        {
            this.Movie = movie;
        }

        public Movie Movie { get; }

        public static MovieListItem FromMovie(Movie movie)
        {
            string title = movie.Titles
                .Where(title => !title.IsOriginal)
                .OrderByDescending(title => title.Priority)
                .First()
                .Name;

            string originalTitle = movie.Titles
                .Where(title => title.IsOriginal)
                .OrderByDescending(title => title.Priority)
                .First()
                .Name;

            var color = IntToColor(movie.Kind.ColorForMovie);

            return new MovieListItem(movie, movie.Entry, title, originalTitle, movie.Year.ToString(), color);
        }
    }
}
