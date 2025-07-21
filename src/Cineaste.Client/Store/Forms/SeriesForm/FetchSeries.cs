namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record FetchSeriesAction(Guid Id);

public sealed record FetchSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class FetchSeriesReducers
{
    [ReducerMethod(typeof(FetchSeriesAction))]
    public static SeriesFormState ReduceFetchSeriesAction(SeriesFormState _) =>
        new() { Fetch = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceFetchSeriesResultAction(
        SeriesFormState state,
        FetchSeriesResultAction action) =>
        action.Handle(
            onSuccess: series =>
                state with { Fetch = ApiCall.Success(), Model = series, SelectedSeriesComponent = null },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem), SelectedSeriesComponent = null });
}

public sealed partial class FetchSeriesEffects(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleFetchSeries(FetchSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.GetSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchSeriesResultAction(result));
    }
}
