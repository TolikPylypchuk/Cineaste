namespace Cineaste.Client.Store.Forms.SeriesForm;

public sealed record UpdateSeriesAction(Guid Id, SeriesRequest Request);

public sealed record UpdateSeriesResultAction(ApiResult<SeriesModel> Result) : ResultAction<SeriesModel>(Result);

public static class UpdateSeriesReducers
{
    [ReducerMethod(typeof(UpdateSeriesAction))]
    public static SeriesFormState ReduceUpdateMovieAction(SeriesFormState state) =>
        state with { Update = ApiCall.InProgress() };

    [ReducerMethod]
    public static SeriesFormState ReduceUpdateMovieResultAction(
        SeriesFormState state,
        UpdateSeriesResultAction action) =>
        action.Handle(
            onSuccess: series => state with { Update = ApiCall.Success(), Model = series },
            onFailure: problem => state with { Update = ApiCall.Failure(problem) });
}

public sealed class UpdateSeriesEffects(ISeriesApi api)
{
    [EffectMethod]
    public async Task HandleUpdateSeries(UpdateSeriesAction action, IDispatcher dispatcher)
    {
        var result = await api.UpdateSeries(action.Id, action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new UpdateSeriesResultAction(result));
    }
}
