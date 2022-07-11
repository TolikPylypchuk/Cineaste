namespace Cineaste.Client.Store.Forms.SeriesForm;

using Cineaste.Client.Store.ListPage;

public sealed record FetchSeriesAction(Guid Id, ImmutableList<ListKindModel> AvailableKinds);

public sealed record FetchSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class FetchSeriesReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceFetchSeriesAction(SeriesFormState _, FetchSeriesAction action) =>
        new() { IsLoading = true, AvailableKinds = action.AvailableKinds };

    [ReducerMethod]
    public static SeriesFormState ReduceSelectItemAction(SeriesFormState state, SelectItemAction action) =>
        action.Item.Id == state.SeriesModel?.Id ? state : new() { IsLoading = true };

    [ReducerMethod]
    public static SeriesFormState ReduceFetchSeriesResultAction(
        SeriesFormState state,
        FetchSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { IsLoading = false, IsLoaded = true, SeriesModel = series },
            onFailure: problem => state with { IsLoading = false, IsLoaded = true, FetchSeriesProblem = problem });
}

[AutoConstructor]
public sealed partial class FetchSeriesEffect
{
    private readonly ISeriesApi api;

    [EffectMethod]
    public async Task HandleFetchSeries(FetchSeriesAction action, IDispatcher dispatcher)
    {
        var result = await this.api.GetSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchSeriesResultAction(result));
    }
}
