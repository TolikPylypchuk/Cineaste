namespace Cineaste.Shared.Mapping;

public static class MappingExtensions
{
    public static ListItemModel ToListItemModel(this MovieModel movie) =>
        new(
            movie.Id,
            ListItemType.Movie,
            movie.Titles.First().Name,
            movie.OriginalTitles.First().Name,
            movie.Year,
            movie.Year,
            movie.ListItemColor,
            movie.ListSequenceNumber,
            movie.FranchiseItem.ToListFranchiseItem());

    public static ListItemModel ToListItemModel(this SeriesModel series) =>
        new(
            series.Id,
            ListItemType.Series,
            series.Titles.First().Name,
            series.OriginalTitles.First().Name,
            series.StartYear,
            series.EndYear,
            series.ListItemColor,
            series.ListSequenceNumber,
            series.FranchiseItem.ToListFranchiseItem());

    public static ListItemModel ToListItemModel(this FranchiseModel franchise) =>
        new(
            franchise.Id,
            ListItemType.Franchise,
            franchise.Titles.First().Name,
            franchise.OriginalTitles.First().Name,
            franchise.Items.Select(item => item.StartYear).Min(),
            franchise.Items.Select(item => item.EndYear).Max(),
            franchise.ListItemColor,
            franchise.ListSequenceNumber,
            franchise.FranchiseItem.ToListFranchiseItem());

    [return: NotNullIfNotNull(nameof(item))]
    public static ListFranchiseItemModel? ToListFranchiseItem(this FranchiseItemInfoModel? item) =>
        item is not null
            ? new(item.ParentFranchiseId, item.SequenceNumber, item.DisplayNumber, item.IsLooselyConnected)
            : null;

    public static ListItemType ToListItemType(this FranchiseItemType type) =>
        type switch
        {
            FranchiseItemType.Movie => ListItemType.Movie,
            FranchiseItemType.Series => ListItemType.Series,
            _ => ListItemType.Franchise,
        };

    public static FranchiseItemType ToFranchiseItemType(this ListItemType type) =>
        type switch
        {
            ListItemType.Movie => FranchiseItemType.Movie,
            ListItemType.Series => FranchiseItemType.Series,
            _ => FranchiseItemType.Franchise,
        };
}
