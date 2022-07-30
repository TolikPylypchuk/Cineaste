namespace Cineaste.Client.Store.Forms.SeriesForm;

using Cineaste.Client.Store.ListPage;

public sealed record FetchSeriesAction(Guid Id, ImmutableList<ListKindModel> AvailableKinds);

public sealed record FetchSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class FetchSeriesReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceFetchSeriesAction(SeriesFormState _, FetchSeriesAction action) =>
        new() { Fetch = ApiCall.InProgress(), AvailableKinds = action.AvailableKinds, SelectedSeriesComponent = null };

    [ReducerMethod]
    public static SeriesFormState ReduceSelectItemAction(SeriesFormState state, SelectItemAction action) =>
        action.Item.Id == state.Model?.Id
            ? state with { SelectedSeriesComponent = null }
            : new() { Fetch = ApiCall.InProgress(), SelectedSeriesComponent = null };

    [ReducerMethod]
    public static SeriesFormState ReduceFetchSeriesResultAction(
        SeriesFormState state,
        FetchSeriesResultAction action) =>
        action.Handle(
            onSuccess: series =>
                state with { Fetch = ApiCall.Success(), Model = series, SelectedSeriesComponent = null },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem), SelectedSeriesComponent = null });
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
