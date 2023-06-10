using Cineaste.Core.Domain;

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

    public static FranchiseUpdateResult Update(
        this Franchise franchise,
        Validated<FranchiseRequest> request,
        IEnumerable<Movie> movies,
        IEnumerable<Series> series,
        IEnumerable<Franchise> franchises)
    {
        franchise.ShowTitles = request.Value.ShowTitles;
        franchise.IsLooselyConnected = request.Value.IsLooselyConnected;
        franchise.ContinueNumbering = request.Value.ContinueNumbering;

        franchise.ReplaceTitles(request.Value);

        var items = request.Value.Items.ToDictionary(item => (item.Id, item.Type), item => item);

        var removedItems = franchise.RemoveMissingItems(items);

        UpdateItems(movies, franchise.FindMovie, franchise.AddMovie, items, FranchiseItemType.Movie);
        UpdateItems(series, franchise.FindSeries, franchise.AddSeries, items, FranchiseItemType.Series);
        UpdateItems(franchises, franchise.FindFranchise, franchise.AddFranchise, items, FranchiseItemType.Franchise);

        return new FranchiseUpdateResult(removedItems);
    }

    private static IReadOnlyList<FranchiseItem> RemoveMissingItems(
        this Franchise franchise,
        IDictionary<(Guid, FranchiseItemType), FranchiseItemRequest> itemsById)
    {
        var items = franchise.Children
            .Where(item =>
            {
                var id = item.Select(m => m.Id.Value, s => s.Id.Value, f => f.Id.Value);

                var type = item.Select(
                    m => FranchiseItemType.Movie,
                    s => FranchiseItemType.Series,
                    f => FranchiseItemType.Franchise);

                return !itemsById.ContainsKey((id, type));
            })
            .ToImmutableList();

        foreach (var item in items)
        {
            item.Do(franchise.RemoveMovie, franchise.RemoveSeries, franchise.RemoveFranchise);
        }

        return items;
    }

    private static void UpdateItems<T>(
        IEnumerable<T> items,
        Func<T, FranchiseItem?> find,
        Func<T, FranchiseItem> add,
        IDictionary<(Guid, FranchiseItemType), FranchiseItemRequest> requestItems,
        FranchiseItemType itemType)
        where T : Entity<T>
    {
        foreach (var item in items)
        {
            var franchiseItem = find(item) ?? add(item);
            var requestItem = requestItems[(item.Id.Value, itemType)];

            franchiseItem.SequenceNumber = requestItem.SequenceNumber;
            franchiseItem.ShouldDisplayNumber = requestItem.ShouldDisplayNumber;
        }
    }

    private static void ReplaceTitles(this Franchise franchise, FranchiseRequest request)
    {
        if (franchise.ShowTitles)
        {
            franchise.ReplaceTitles(
                request.Titles.OrderBy(title => title.Priority).Select(title => title.Name),
                isOriginal: false);

            franchise.ReplaceTitles(
                request.OriginalTitles.OrderBy(title => title.Priority).Select(title => title.Name),
                isOriginal: true);
        } else
        {
            franchise.ReplaceTitles(Enumerable.Empty<string>(), isOriginal: false);
            franchise.ReplaceTitles(Enumerable.Empty<string>(), isOriginal: true);
        }
    }
}
