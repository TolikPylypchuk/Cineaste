namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record AddSeriesAction(SeriesRequest Request);

public sealed record AddSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class AddSeriesReducers
{
    [ReducerMethod(typeof(AddSeriesAction))]
    public static SeriesFormState ReduceAddSeriesAction(SeriesFormState state) =>
        state with { Add = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceAddSeriesResultAction(
        SeriesFormState state,
        AddSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { Add = ApiCall.Success(), Model = series },
            onFailure: problem => state with { Add = ApiCall.Failure(problem) });
}

public sealed class AddSeriesEffect(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleAddSeries(AddSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.AddSeries(action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new AddSeriesResultAction(result));
    }
}
