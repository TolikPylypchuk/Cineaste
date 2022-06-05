namespace Cineaste.Server.Services;

public sealed class ListMapper : IListMapper
{
    public ListModel MapToListModel(CineasteList list) =>
        new(
            list.Id.Value,
            list.Name,
            list.Handle,
            this.ToConfigurationModel(list.Configuration),
            list.Movies.Select(this.ToListItemModel).ToList(),
            list.Series.Select(this.ToListItemModel).ToList(),
            list.Franchises.Select(this.ToListItemModel).ToList(),
            list.MovieKinds.Select(this.ToListKindModel).ToList(),
            list.SeriesKinds.Select(this.ToListKindModel).ToList());

    private ListConfigurationModel ToConfigurationModel(ListConfiguration config) =>
        new(
            config.Culture.ToString(),
            config.DefaultSeasonTitle,
            config.DefaultSeasonOriginalTitle,
            config.SortingConfiguration.DefaultFirstSortOrder,
            config.SortingConfiguration.DefaultFirstSortDirection,
            config.SortingConfiguration.DefaultSecondSortOrder,
            config.SortingConfiguration.DefaultSecondSortDirection);

    private ListItemModel ToListItemModel(Movie movie) =>
        new(
            movie.Id.Value,
            ListItemType.Movie,
            true,
            movie.FranchiseItem.GetDisplayNumber(),
            movie.Title.Name,
            movie.OriginalTitle.Name,
            movie.Year,
            movie.Year,
            movie.GetActiveColor().HexValue,
            this.ToFranchiseItemModel(movie.FranchiseItem));

    private ListItemModel ToListItemModel(Series series) =>
        new(
            series.Id.Value,
            ListItemType.Series,
            true,
            series.FranchiseItem.GetDisplayNumber(),
            series.Title.Name,
            series.OriginalTitle.Name,
            series.StartYear,
            series.EndYear,
            series.GetActiveColor().HexValue,
            this.ToFranchiseItemModel(series.FranchiseItem));

    private ListItemModel ToListItemModel(Franchise franchise) =>
        new(
            franchise.Id.Value,
            ListItemType.Franchise,
            franchise.ShowTitles,
            franchise.FranchiseItem.GetDisplayNumber(),
            franchise.ShowTitles && franchise.Title is not null
                ? $"{franchise.Title.Name}:"
                : String.Empty,
            franchise.ShowTitles && franchise.OriginalTitle is not null
                ? $"{franchise.OriginalTitle.Name}:"
                : String.Empty,
            franchise.GetFirstChild()?.GetStartYear() ?? 0,
            franchise.GetLastChild()?.GetEndYear() ?? 0,
            franchise.GetActiveColor()?.HexValue ?? String.Empty,
            this.ToFranchiseItemModel(franchise.FranchiseItem));

    private ListKindModel ToListKindModel<TKind>(Kind<TKind> kind)
        where TKind : Kind<TKind> =>
        new(
            kind.Id.Value,
            kind.Name,
            kind.WatchedColor.HexValue,
            kind.NotWatchedColor.HexValue,
            kind.NotReleasedColor.HexValue,
            kind is MovieKind ? ListKindTarget.Movie : ListKindTarget.Series);

    [return: NotNullIfNotNull("item")]
    private ListFranchiseItemModel? ToFranchiseItemModel(FranchiseItem? item) =>
        item is not null ? new(item.ParentFranchise.Id.Value, item.SequenceNumber) : null;
}
