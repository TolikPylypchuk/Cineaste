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

    public static Franchise ToFranchise(
        this Validated<FranchiseRequest> request,
        Id<Franchise> id,
        IReadOnlyDictionary<Id<Movie>, Movie> moviesById,
        IReadOnlyDictionary<Id<Series>, Series> seriesById,
        IReadOnlyDictionary<Id<Franchise>, Franchise> franchisesById)
    {
        var franchise = new Franchise(
            id,
            request.Value.ToTitles(),
            request.Value.ShowTitles,
            request.Value.IsLooselyConnected,
            request.Value.ContinueNumbering);

        foreach (var item in request.Value.Items.OrderBy(item => item.SequenceNumber))
        {
            switch (item.Type)
            {
                case FranchiseItemType.Movie:
                    franchise.AddMovie(moviesById[Id.Create<Movie>(item.Id)]);
                    break;
                case FranchiseItemType.Series:
                    franchise.AddSeries(seriesById[Id.Create<Series>(item.Id)]);
                    break;
                case FranchiseItemType.Franchise:
                    franchise.AddFranchise(franchisesById[Id.Create<Franchise>(item.Id)]);
                    break;
            }
        }

        return franchise;
    }

    private static ImmutableList<FranchiseItemModel> ToItemModels(this IEnumerable<FranchiseItem> items) =>
        items
            .OrderBy(item => item.SequenceNumber)
            .Select(item => item.Select(
                movie => new FranchiseItemModel(
                    movie.Id.Value,
                    item.SequenceNumber,
                    item.ShouldDisplayNumber,
                    movie.Title.Name,
                    movie.Year,
                    movie.Year,
                    FranchiseItemType.Movie),
                series => new FranchiseItemModel(
                    series.Id.Value,
                    item.SequenceNumber,
                    item.ShouldDisplayNumber,
                    series.Title.Name,
                    series.StartYear,
                    series.EndYear,
                    FranchiseItemType.Series),
                franchise => new FranchiseItemModel(
                    franchise.Id.Value,
                    item.SequenceNumber,
                    item.ShouldDisplayNumber,
                    franchise.ActualTitle?.Name ?? String.Empty,
                    franchise.StartYear ?? 0,
                    franchise.EndYear ?? 0,
                    FranchiseItemType.Franchise)))
            .ToImmutableList();
}
