namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record UpdateMovieAction(Guid Id, MovieRequest Request);

public sealed record UpdateMovieResultAction(ApiResult<MovieModel> Result) : ResultAction<MovieModel>(Result);

public static class UpdateMovieReducers
{
    [ReducerMethod(typeof(UpdateMovieAction))]
    public static MovieFormState ReduceUpdateMovieAction(MovieFormState state) =>
        state with { Update = ApiCall.InProgress() };

    [ReducerMethod]
    public static MovieFormState ReduceUpdateMovieResultAction(MovieFormState state, UpdateMovieResultAction action) =>
        action.Handle(
            onSuccess: movie => state with { Update = ApiCall.Success(), Model = movie },
            onFailure: problem => state with { Update = ApiCall.Failure(problem) });
}

public sealed class UpdateMovieEffects(IMovieApi api)
{
    [EffectMethod]
    public async Task HandleUpdateMovie(UpdateMovieAction action, IDispatcher dispatcher)
    {
        var result = await api.UpdateMovie(action.Id, action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new UpdateMovieResultAction(result));
    }
}
