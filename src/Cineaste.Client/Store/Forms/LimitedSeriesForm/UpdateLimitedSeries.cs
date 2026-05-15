namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

public sealed record UpdateLimitedSeriesAction(Guid Id, LimitedSeriesRequest Request);

public sealed record UpdateLimitedSeriesResultAction(ApiResult<LimitedSeriesModel> Result) : ResultAction<LimitedSeriesModel>(Result);

public static class UpdateLimitedSeriesReducers
{
    [ReducerMethod(typeof(UpdateLimitedSeriesAction))]
    public static LimitedSeriesFormState ReduceUpdateLimitedSeriesAction(LimitedSeriesFormState state) =>
        state with { Update = ApiCall.InProgress() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceUpdateLimitedSeriesResultAction(LimitedSeriesFormState state, UpdateLimitedSeriesResultAction action) =>
        action.Handle(
            onSuccess: LimitedSeries => state with { Update = ApiCall.Success(), Model = LimitedSeries },
            onFailure: problem => state with { Update = ApiCall.Failure(problem) });
}

public sealed class UpdateLimitedSeriesEffects(ILimitedSeriesApi api)
{
    [EffectMethod]
    public async Task HandleUpdateLimitedSeries(UpdateLimitedSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.UpdateLimitedSeries(action.Id, action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new UpdateLimitedSeriesResultAction(result));
    }
}
