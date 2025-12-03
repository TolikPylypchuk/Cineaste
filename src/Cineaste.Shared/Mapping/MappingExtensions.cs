namespace Cineaste.Shared.Mapping;

public static class MappingExtensions
{
    extension(MovieModel movie)
    {
        public ListItemModel ToListItemModel() =>
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
    }

    extension(SeriesModel series)
    {
        public ListItemModel ToListItemModel() =>
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
    }

    extension(FranchiseModel franchise)
    {
        public ListItemModel ToListItemModel() =>
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
    }

    extension(FranchiseItemInfoModel? item)
    {
        [return: NotNullIfNotNull(nameof(item))]
        public ListFranchiseItemModel? ToListFranchiseItem() =>
            item is not null
                ? new(item.ParentFranchiseId, item.SequenceNumber, item.DisplayNumber, item.IsLooselyConnected)
                : null;
    }

    extension(FranchiseItemType type)
    {
        public ListItemType ToListItemType() =>
            type switch
            {
                FranchiseItemType.Movie => ListItemType.Movie,
                FranchiseItemType.Series => ListItemType.Series,
                _ => ListItemType.Franchise,
            };
    }

    extension(ListItemType type)
    {
        public FranchiseItemType ToFranchiseItemType() =>
            type switch
            {
                ListItemType.Movie => FranchiseItemType.Movie,
                ListItemType.Series => FranchiseItemType.Series,
                _ => FranchiseItemType.Franchise,
            };
    }
}
