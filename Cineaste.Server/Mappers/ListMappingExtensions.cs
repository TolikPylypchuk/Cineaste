namespace Cineaste.Server.Mappers;

public static class ListMappingExtensions
{
    public static ListModel ToListModel(this CineasteList list) =>
        new(
            list.Id.Value,
            list.Name,
            list.Handle,
            list.Configuration.ToConfigurationModel(),
            list.Movies.Select(ToListItemModel).ToList(),
            list.Series.Select(ToListItemModel).ToList(),
            list.Franchises.Select(ToListItemModel).ToList(),
            list.MovieKinds.Select(kind => kind.ToListKindModel()).ToList(),
            list.SeriesKinds.Select(kind => kind.ToListKindModel()).ToList());

    public static ListConfigurationModel ToConfigurationModel(this ListConfiguration config) =>
        new(
            config.Culture.ToString(),
            config.DefaultSeasonTitle,
            config.DefaultSeasonOriginalTitle,
            config.SortingConfiguration.DefaultFirstSortOrder,
            config.SortingConfiguration.DefaultFirstSortDirection,
            config.SortingConfiguration.DefaultSecondSortOrder,
            config.SortingConfiguration.DefaultSecondSortDirection);

    public static ListItemModel ToListItemModel(this Movie movie) =>
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
            movie.FranchiseItem.ToFranchiseItemModel());

    public static ListItemModel ToListItemModel(this Series series) =>
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
            series.FranchiseItem.ToFranchiseItemModel());

    public static ListItemModel ToListItemModel(this Franchise franchise) =>
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
            franchise.FranchiseItem.ToFranchiseItemModel());

    [return: NotNullIfNotNull("item")]
    private static ListFranchiseItemModel? ToFranchiseItemModel(this FranchiseItem? item) =>
        item is not null ? new(item.ParentFranchise.Id.Value, item.SequenceNumber) : null;
}
