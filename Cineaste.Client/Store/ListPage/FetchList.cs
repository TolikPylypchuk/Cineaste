namespace Cineaste.Client.Store.ListPage;

using Nito.Comparers;

public sealed record FetchListAction(string Handle);

public sealed record FetchListResultAction(ApiResult<ListModel> Result) : ResultAction<ListModel>(Result);

public static class FetchListReducers
{
    [ReducerMethod(typeof(FetchListAction))]
    public static ListPageState ReduceFetchListsAction(ListPageState _) =>
        new() { IsLoading = true };

    [ReducerMethod]
    public static ListPageState ReduceFetchListsResultAction(ListPageState state, FetchListResultAction action) =>
        action.Handle(
            onSuccess: list =>
                state with
                {
                    IsLoading = false,
                    IsLoaded = true,
                    Id = list.Id,
                    Name = list.Name,
                    Items = SortItems(list),
                    AvailableMovieKinds = list.MovieKinds
                        .Select(model => model.ToSimpleModel())
                        .ToImmutableList(),
                    AvailableSeriesKinds = list.SeriesKinds
                        .Select(model => model.ToSimpleModel())
                        .ToImmutableList(),
                },
            onFailure: problem => new() { IsLoaded = true, Problem = problem });

    private static ImmutableSortedSet<ListItemModel> SortItems(ListModel list)
    {
        var culture = CultureInfo.GetCultureInfo(list.Config.Culture);

        var comparerByYear = ComparerBuilder.For<ListItemModel>()
            .OrderBy(item => item.StartYear, descending: false)
            .ThenBy(item => item.EndYear, descending: false);

        var itemsById = list.Movies
            .Concat(list.Series)
            .Concat(list.Franchises)
            .ToImmutableDictionary(item => item.Id, item => item);

        var comparer = new ListItemTitleComparer(
            culture,
            comparerByYear,
            id => itemsById[id],
            item => item.Title);

        return list.Movies
            .Concat(list.Series)
            .Concat(list.Franchises)
            .ToImmutableSortedSet(comparer);
    }
}

[AutoConstructor]
public sealed partial class FetchListEffect
{
    private readonly IListApi api;

    [EffectMethod]
    public async Task HandleFetchListAction(FetchListAction action, IDispatcher dispatcher)
    {
        var result = await this.api.GetList(action.Handle).ToApiResultAsync();
        dispatcher.Dispatch(new FetchListResultAction(result));
    }
}
