namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

public sealed record AddLimitedSeriesAction(LimitedSeriesRequest Request);

public sealed record AddLimitedSeriesResultAction(ApiResult<LimitedSeriesModel> Result)
    : ResultAction<LimitedSeriesModel>(Result);

public static class AddLimitedSeriesReducers
{
    [ReducerMethod(typeof(AddLimitedSeriesAction))]
    public static LimitedSeriesFormState ReduceAddLimitedSeriesAction(LimitedSeriesFormState state) =>
        state with { Add = ApiCall.InProgress() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceAddLimitedSeriesResultAction(
        LimitedSeriesFormState state,
        AddLimitedSeriesResultAction action) =>
        action.Handle(
            onSuccess: limitedSeries => state with
            {
                InitialParentFranchiseId = null,
                Add = ApiCall.Success(),
                Model = limitedSeries
            },
            onFailure: problem => state with { Add = ApiCall.Failure(problem) });
}

public sealed class AddLimitedSeriesEffects(ILimitedSeriesApi api)
{
    [EffectMethod]
    public async Task HandleAddLimitedSeries(AddLimitedSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.AddLimitedSeries(action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new AddLimitedSeriesResultAction(result));
    }
}
