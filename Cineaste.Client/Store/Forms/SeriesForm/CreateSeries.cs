namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record CreateSeriesAction(SeriesRequest Request);

public sealed record CreateSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class CreateSeriesReducers
{
    [ReducerMethod(typeof(CreateSeriesAction))]
    public static SeriesFormState ReduceCreateSeriesAction(SeriesFormState state) =>
        state with { Create = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceCreateSeriesResultAction(
        SeriesFormState state,
        CreateSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { Create = ApiCall.Success(), Model = series },
            onFailure: problem => state with { Create = ApiCall.Failure(problem) });
}

public sealed class CreateSeriesEffect(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleCreateSeries(CreateSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.CreateSeries(action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new CreateSeriesResultAction(result));
    }
}
