namespace Cineaste.Application.Mapping;

public static class ListMappingExtensions
{
    extension(CineasteList list)
    {
        public ListModel ToListModel() =>
            new(
                list.Id.Value,
                list.Configuration.ToConfigurationModel(),
                [.. list.MovieKinds.Select(kind => kind.ToListKindModel())],
                [.. list.SeriesKinds.Select(kind => kind.ToListKindModel())]);
    }

    extension(ListConfiguration config)
    {
        public ListConfigurationModel ToConfigurationModel() =>
            new(
                config.Culture.ToString(),
                config.DefaultSeasonTitle,
                config.DefaultSeasonOriginalTitle,
                config.SortingConfiguration.FirstSortOrder,
                config.SortingConfiguration.SecondSortOrder,
                config.SortingConfiguration.SortDirection);
    }

    extension(ListItem item)
    {
        public ListItemModel ToListItemModel() =>
            item.Select(item.ToListItemModel, item.ToListItemModel, item.ToListItemModel);

        public ListItemModel ToListItemModel(Movie movie) =>
            new(
                movie.Id.Value,
                ListItemType.Movie,
                movie.Title.Name,
                movie.OriginalTitle.Name,
                item.StartYear,
                item.EndYear,
                item.ActiveColor?.HexValue ?? String.Empty,
                item.SequenceNumber,
                movie.FranchiseItem.ToFranchiseItemModel());

        public ListItemModel ToListItemModel(Series series) =>
            new(
                series.Id.Value,
                ListItemType.Series,
                series.Title.Name,
                series.OriginalTitle.Name,
                item.StartYear,
                item.EndYear,
                item.ActiveColor?.HexValue ?? String.Empty,
                item.SequenceNumber,
                series.FranchiseItem.ToFranchiseItemModel());

        public ListItemModel ToListItemModel(Franchise franchise) =>
            new(
                franchise.Id.Value,
                ListItemType.Franchise,
                franchise.Title.Name,
                franchise.OriginalTitle.Name,
                item.StartYear,
                item.EndYear,
                item.ActiveColor?.HexValue ?? String.Empty,
                item.SequenceNumber,
                franchise.FranchiseItem.ToFranchiseItemModel());
    }

    extension(FranchiseItem? item)
    {
        [return: NotNullIfNotNull(nameof(item))]
        private ListFranchiseItemModel? ToFranchiseItemModel() =>
            item is not null
                ? new(
                    item.ParentFranchise.Id.Value,
                    item.SequenceNumber,
                    item.DisplayNumber,
                    item.ParentFranchise.IsLooselyConnected)
                : null;
    }
}
