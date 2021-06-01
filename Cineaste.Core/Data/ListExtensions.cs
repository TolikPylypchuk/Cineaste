using System.Collections.Generic;
using System.Linq;

using Cineaste.Core.ListItems;
using Cineaste.Data;
using Cineaste.Data.Models;

namespace Cineaste.Core.Data
{
    public static class ListExtensions
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
