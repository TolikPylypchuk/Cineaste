using System.Collections.Generic;
using System.Linq;

using MovieList.Core.ListItems;
using MovieList.Data;
using MovieList.Data.Models;

namespace MovieList.Core.Data
{
    public static class MovieListExtensions
    {
        public static List<ListItem> ToListItems(this EntireList list) =>
            list.Movies.Select(movie => new MovieListItem(movie))
                .OfType<ListItem>()
                .Concat(list.Series.Select(series => new SeriesListItem(series)))
                .Concat(list.Franchise.Select(franchise => new FranchiseListItem(franchise)))
                .ToList();

        public static List<FranchiseEntry> ToEntries(this EntireList list) =>
            list.Movies.Select(movie => new FranchiseEntry { Movie = movie })
                .Concat(list.Series.Select(series => new FranchiseEntry { Series = series }))
                .Concat(list.Franchise.Select(franchise => new FranchiseEntry { Franchise = franchise }))
                .ToList();
    }
}
