namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record FetchSeriesAction(
    Guid Id,
    ImmutableList<ListKindModel> AvailableKinds,
    ListConfigurationModel ListConfiguration);

public sealed record FetchSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class FetchSeriesReducers
{
    [ReducerMethod]
    public static SeriesFormState ReduceFetchSeriesAction(SeriesFormState _, FetchSeriesAction action) =>
        new()
        {
            Fetch = ApiCall.InProgress(),
            AvailableKinds = action.AvailableKinds,
            ListConfiguration = action.ListConfiguration
        };

    [ReducerMethod]
    public static SeriesFormState ReduceFetchSeriesResultAction(
        SeriesFormState state,
        FetchSeriesResultAction action) =>
        action.Handle(
            onSuccess: series =>
                state with { Fetch = ApiCall.Success(), Model = series, SelectedSeriesComponent = null },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem), SelectedSeriesComponent = null });
}

public sealed partial class FetchSeriesEffect(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleFetchSeries(FetchSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.GetSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchSeriesResultAction(result));
    }
}
