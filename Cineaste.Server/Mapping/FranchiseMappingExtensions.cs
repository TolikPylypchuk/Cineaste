namespace Cineaste.Server.Mapping;

public static class FranchiseMappingExtensions
{
    public static FranchiseModel ToFranchiseModel(this Franchise franchise) =>
        new(
            franchise.Id.Value,
            franchise.Titles.ToTitleModels(isOriginal: false),
            franchise.Titles.ToTitleModels(isOriginal: true),
            franchise.Children.ToItemModels(),
            franchise.ShowTitles,
            franchise.IsLooselyConnected,
            franchise.ContinueNumbering);

    private static ImmutableList<FranchiseItemModel> ToItemModels(this IEnumerable<FranchiseItem> items) =>
        items
            .OrderBy(item => item.SequenceNumber)
            .Select(item => item.Select(
                movie => new FranchiseItemModel(
                    movie.Id.Value,
                    item.SequenceNumber,
                    movie.Title.Name,
                    movie.Year,
                    movie.Year,
                    FranchiseItemType.Movie),
                series => new FranchiseItemModel(
                    series.Id.Value,
                    item.SequenceNumber,
                    series.Title.Name,
                    series.StartYear,
                    series.EndYear,
                    FranchiseItemType.Series),
                franchise => new FranchiseItemModel(
                    franchise.Id.Value,
                    item.SequenceNumber,
                    franchise.ActualTitle?.Name ?? String.Empty,
                    franchise.StartYear ?? 0,
                    franchise.EndYear ?? 0,
                    FranchiseItemType.Franchise)))
            .ToImmutableList();
}
