namespace Cineaste.Mapping;

public static class ListMappingExtensions
{
    public static ListModel ToListModel(this CineasteList list) =>
        new(
            list.Id.Value,
            list.Configuration.ToConfigurationModel(),
            [.. list.MovieKinds.Select(kind => kind.ToListKindModel())],
            [.. list.SeriesKinds.Select(kind => kind.ToListKindModel())]);

    public static ListConfigurationModel ToConfigurationModel(this ListConfiguration config) =>
        new(
            config.Culture.ToString(),
            config.DefaultSeasonTitle,
            config.DefaultSeasonOriginalTitle,
            config.SortingConfiguration.FirstSortOrder,
            config.SortingConfiguration.SecondSortOrder,
            config.SortingConfiguration.SortDirection);

    public static ListItemModel ToListItemModel(this ListItem item) =>
        item.Select(item.ToListItemModel, item.ToListItemModel, item.ToListItemModel);

    public static ListItemModel ToListItemModel(this ListItem item, Movie movie) =>
        new(
            movie.Id.Value,
            ListItemType.Movie,
            movie.Title.Name,
            movie.OriginalTitle.Name,
            item.StartYear,
            item.EndYear,
            item.ActiveColor?.HexValue ?? String.Empty,
            movie.FranchiseItem.ToFranchiseItemModel());

    public static ListItemModel ToListItemModel(this ListItem item, Series series) =>
        new(
            series.Id.Value,
            ListItemType.Series,
            series.Title.Name,
            series.OriginalTitle.Name,
            item.StartYear,
            item.EndYear,
            item.ActiveColor?.HexValue ?? String.Empty,
            series.FranchiseItem.ToFranchiseItemModel());

    public static ListItemModel ToListItemModel(this ListItem item, Franchise franchise) =>
        new(
            franchise.Id.Value,
            ListItemType.Franchise,
            franchise.ShowTitles && franchise.Title is not null
                ? $"{franchise.Title.Name}:"
                : String.Empty,
            franchise.ShowTitles && franchise.OriginalTitle is not null
                ? $"{franchise.OriginalTitle.Name}:"
                : String.Empty,
            item.StartYear,
            item.EndYear,
            item.ActiveColor?.HexValue ?? String.Empty,
            franchise.FranchiseItem.ToFranchiseItemModel());

    [return: NotNullIfNotNull(nameof(item))]
    private static ListFranchiseItemModel? ToFranchiseItemModel(this FranchiseItem? item) =>
        item is not null
            ? new(
                item.ParentFranchise.Id.Value,
                item.SequenceNumber,
                item.DisplayNumber,
                item.ParentFranchise.IsLooselyConnected)
            : null;
}
