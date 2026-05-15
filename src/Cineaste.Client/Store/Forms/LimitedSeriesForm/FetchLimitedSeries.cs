namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

public sealed record FetchLimitedSeriesAction(Guid Id);

public sealed record FetchLimitedSeriesResultAction(ApiResult<LimitedSeriesModel> Result)
    : ResultAction<LimitedSeriesModel>(Result);

public static class FetchLimitedSeriesReducers
{
    [ReducerMethod(typeof(FetchLimitedSeriesAction))]
    public static LimitedSeriesFormState ReduceFetchLimitedSeriesAction(LimitedSeriesFormState _) =>
        new() { Fetch = ApiCall.InProgress() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceFetchLimitedSeriesResultAction(
        LimitedSeriesFormState state,
        FetchLimitedSeriesResultAction action) =>
        action.Handle(
            onSuccess: limitedSeries => state with { Fetch = ApiCall.Success(), Model = limitedSeries },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem) });
}

public sealed class FetchLimitedSeriesEffects(ILimitedSeriesApi api)
{
    [EffectMethod]
    public async Task HandleFetchLimitedSeries(FetchLimitedSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.GetLimitedSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchLimitedSeriesResultAction(result));
    }
}
