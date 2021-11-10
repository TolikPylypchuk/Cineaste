namespace Cineaste.Core.Data;

public static class ListExtensions
{
    public static List<ListItem> ToListItems(this DataList list) =>
        list.Movies.Select(movie => new MovieListItem(movie))
            .OfType<ListItem>()
            .Concat(list.Series.Select(series => new SeriesListItem(series)))
            .Concat(list.Franchises.Select(franchise => new FranchiseListItem(franchise)))
            .ToList();

    public static List<FranchiseEntry> ToEntries(this DataList list) =>
        list.Movies.Select(movie => new FranchiseEntry { Movie = movie })
            .Concat(list.Series.Select(series => new FranchiseEntry { Series = series }))
            .Concat(list.Franchises.Select(franchise => new FranchiseEntry { Franchise = franchise }))
            .ToList();
}
