namespace Cineaste.Mapping;

public static class FranchiseMappingExtensions
{
    public static FranchiseModel ToFranchiseModel(this Franchise franchise) =>
        new(
            franchise.Id.Value,
            franchise.AllTitles.ToTitleModels(isOriginal: false),
            franchise.AllTitles.ToTitleModels(isOriginal: true),
            franchise.Children.ToItemModels(),
            franchise.KindSource == FranchiseKindSource.Movie
                ? franchise.MovieKind.ToListKindModel()
                : franchise.SeriesKind.ToListKindModel(),
            franchise.KindSource,
            franchise.ShowTitles,
            franchise.IsLooselyConnected,
            franchise.ContinueNumbering,
            franchise.GetActiveColor()?.HexValue ?? String.Empty,
            franchise.ListItem?.SequenceNumber ?? 0,
            franchise.FranchiseItem.ToFranchiseItemInfoModel());

    [return: NotNullIfNotNull(nameof(item))]
    public static FranchiseItemInfoModel? ToFranchiseItemInfoModel(this FranchiseItem? item) =>
        item is not null
            ? new(
                item.ParentFranchise.Id.Value,
                item.SequenceNumber,
                item.DisplayNumber,
                item.IsFirst(),
                item.IsLast(),
                item.ParentFranchise.IsLooselyConnected)
            : null;

    public static Franchise ToFranchise(
        this Validated<FranchiseRequest> request,
        Id<Franchise> id,
        MovieKind movieKind,
        SeriesKind seriesKind,
        IReadOnlyDictionary<Id<Movie>, Movie> moviesById,
        IReadOnlyDictionary<Id<Series>, Series> seriesById,
        IReadOnlyDictionary<Id<Franchise>, Franchise> franchisesById)
    {
        var franchise = new Franchise(
            id,
            request.Value.ToTitles(),
            movieKind,
            seriesKind,
            request.Value.KindSource,
            request.Value.ShowTitles,
            request.Value.IsLooselyConnected,
            request.Value.ContinueNumbering);

        foreach (var item in request.Value.Items.OrderBy(item => item.SequenceNumber))
        {
            switch (item.Type)
            {
                case FranchiseItemType.Movie:
                    franchise.AddMovie(moviesById[Id.For<Movie>(item.Id)], item.ShouldDisplayNumber);
                    break;
                case FranchiseItemType.Series:
                    franchise.AddSeries(seriesById[Id.For<Series>(item.Id)], item.ShouldDisplayNumber);
                    break;
                case FranchiseItemType.Franchise:
                    franchise.AddFranchise(franchisesById[Id.For<Franchise>(item.Id)], item.ShouldDisplayNumber);
                    break;
            }
        }

        franchise.SetListItemProperties();

        return franchise;
    }

    private static ImmutableList<FranchiseItemModel> ToItemModels(this IEnumerable<FranchiseItem> items) =>
        [.. items
            .OrderBy(item => item.SequenceNumber)
            .Select(item => item.Select(
                movie => new FranchiseItemModel(
                    movie.Id.Value,
                    item.SequenceNumber,
                    item.DisplayNumber,
                    movie.Title.Name,
                    movie.Year,
                    movie.Year,
                    FranchiseItemType.Movie),
                series => new FranchiseItemModel(
                    series.Id.Value,
                    item.SequenceNumber,
                    item.DisplayNumber,
                    series.Title.Name,
                    series.StartYear,
                    series.EndYear,
                    FranchiseItemType.Series),
                franchise => new FranchiseItemModel(
                    franchise.Id.Value,
                    item.SequenceNumber,
                    item.DisplayNumber,
                    franchise.Title.Name,
                    franchise.StartYear,
                    franchise.EndYear,
                    FranchiseItemType.Franchise)))];

    public static FranchiseUpdateResult Update(
        this Franchise franchise,
        Validated<FranchiseRequest> request,
        MovieKind movieKind,
        SeriesKind seriesKind,
        IEnumerable<Movie> movies,
        IEnumerable<Series> series,
        IEnumerable<Franchise> franchises)
    {
        franchise.MovieKind = movieKind;
        franchise.SeriesKind = seriesKind;
        franchise.KindSource = request.Value.KindSource;

        franchise.ShowTitles = request.Value.ShowTitles;
        franchise.IsLooselyConnected = request.Value.IsLooselyConnected;
        franchise.ContinueNumbering = request.Value.ContinueNumbering;

        franchise.ReplaceTitles(request.Value);

        var items = request.Value.Items.ToDictionary(item => (item.Id, item.Type), item => item);

        var removedItems = franchise.RemoveMissingItems(items);

        UpdateItems(movies, franchise.FindMovie, franchise.AddMovie, items, FranchiseItemType.Movie);
        UpdateItems(series, franchise.FindSeries, franchise.AddSeries, items, FranchiseItemType.Series);
        UpdateItems(franchises, franchise.FindFranchise, franchise.AddFranchise, items, FranchiseItemType.Franchise);

        franchise.SetSequenceNumbersForChildren();
        franchise.SetListItemProperties();

        return new FranchiseUpdateResult(removedItems);
    }

    private static ImmutableList<FranchiseItem> RemoveMissingItems(
        this Franchise franchise,
        Dictionary<(Guid, FranchiseItemType), FranchiseItemRequest> itemsById)
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
            item.SetListItemProperties();
        }

        return items;
    }

    private static void UpdateItems<T>(
        IEnumerable<T> items,
        Func<T, FranchiseItem?> find,
        Func<T, bool, FranchiseItem> add,
        Dictionary<(Guid, FranchiseItemType), FranchiseItemRequest> requestItems,
        FranchiseItemType itemType)
        where T : Entity<T>
    {
        foreach (var item in items)
        {
            var requestItem = requestItems[(item.Id.Value, itemType)];
            var franchiseItem = find(item) ?? add(item, requestItem.ShouldDisplayNumber);

            franchiseItem.SequenceNumber = requestItem.SequenceNumber;

            if (requestItem.ShouldDisplayNumber)
            {
                franchiseItem.DisplayNumber = franchiseItem.SequenceNumber;
            } else
            {
                franchiseItem.DisplayNumber = null;
            }
        }
    }

    private static void ReplaceTitles(this Franchise franchise, FranchiseRequest request)
    {
        if (franchise.ShowTitles)
        {
            franchise.ReplaceTitles(
                request.Titles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: false);

            franchise.ReplaceTitles(
                request.OriginalTitles.OrderBy(title => title.SequenceNumber).Select(title => title.Name),
                isOriginal: true);
        } else
        {
            franchise.ReplaceTitles([], isOriginal: false);
            franchise.ReplaceTitles([], isOriginal: true);
        }
    }

    private static void SetListItemProperties(this Franchise franchise)
    {
        franchise.ListItem?.SetProperties(franchise);

        foreach (var item in franchise.Children)
        {
            item.SetListItemProperties();
        }
    }

    private static void SetListItemProperties(this FranchiseItem item) =>
        item.Do(
            movie => movie.ListItem?.SetProperties(movie),
            series => series.ListItem?.SetProperties(series),
            franchise => franchise.ListItem?.SetProperties(franchise));
}
