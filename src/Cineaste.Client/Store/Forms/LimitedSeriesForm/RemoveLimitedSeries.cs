namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

public record RemoveLimitedSeriesAction(Guid Id);

public record RemoveLimitedSeriesResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class RemoveLimitedSeriesReducers
{
    [ReducerMethod(typeof(RemoveLimitedSeriesAction))]
    public static LimitedSeriesFormState ReduceRemoveLimitedSeriesAction(LimitedSeriesFormState state) =>
        state with { Remove = ApiCall.InProgress() };

    [ReducerMethod]
    public static LimitedSeriesFormState ReduceRemoveLimitedSeriesResultAction(
        LimitedSeriesFormState state,
        RemoveLimitedSeriesResultAction action) =>
        action.Handle(
            onSuccess: () => new LimitedSeriesFormState(),
            onFailure: problem => state with { Remove = ApiCall.Failure(problem) });
}

public sealed class RemoveLimitedSeriesEffects(ILimitedSeriesApi api)
{
    [EffectMethod]
    public async Task HandleRemoveLimitedSeriesAction(RemoveLimitedSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveLimitedSeries(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveLimitedSeriesResultAction(action.Id, result));
    }
}
