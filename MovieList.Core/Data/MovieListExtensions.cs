using System.Collections.Generic;
using System.Linq;

using MovieList.ListItems;

namespace MovieList.Data
{
    public static class MovieListExtensions
    {
        public static List<ListItem> ToListItems(this MovieList list)
            => list.Movies.Select(movie => new MovieListItem(movie))
                .Cast<ListItem>()
                .Concat(list.Series.Select(series => new SeriesListItem(series)))
                .Concat(list.MovieSeries.Select(movieSeries => new MovieSeriesListItem(movieSeries)))
                .ToList();
    }
}
