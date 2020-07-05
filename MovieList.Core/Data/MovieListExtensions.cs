using System.Collections.Generic;
using System.Linq;

using MovieList.Data.Models;
using MovieList.ListItems;

namespace MovieList.Data
{
    public static class MovieListExtensions
    {
        public static List<ListItem> ToListItems(this MovieList list)
            => list.Movies.Select(movie => new MovieListItem(movie))
                .Cast<ListItem>()
                .Concat(list.Series.Select(series => new SeriesListItem(series)))
                .Concat(list.Franchise.Select(franchise => new FranchiseListItem(franchise)))
                .ToList();

        public static List<FranchiseEntry> ToEntries(this MovieList list)
            => list.Movies.Select(movie => new FranchiseEntry { Movie = movie })
                .Concat(list.Series.Select(series => new FranchiseEntry { Series = series }))
                .Concat(list.Franchise.Select(franchise => new FranchiseEntry { Franchise = franchise }))
                .ToList();
    }
}
