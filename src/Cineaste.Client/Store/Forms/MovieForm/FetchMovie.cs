namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record FetchMovieAction(Guid Id);

public sealed record FetchMovieResultAction(ApiResult<MovieModel> Result) : ResultAction<MovieModel>(Result);

public static class FetchMovieReducers
{
    [ReducerMethod(typeof(FetchMovieAction))]
    public static MovieFormState ReduceFetchMovieAction(MovieFormState _) =>
        new() { Fetch = ApiCall.InProgress() };

    [ReducerMethod]
    public static MovieFormState ReduceFetchMovieResultAction(MovieFormState state, FetchMovieResultAction action) =>
        action.Handle(
            onSuccess: movie => state with { Fetch = ApiCall.Success(), Model = movie },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem) });
}

public sealed class FetchMovieEffects(IMovieApi api)
{
    [EffectMethod]
    public async Task HandleFetchMovie(FetchMovieAction action, IDispatcher dispatcher)
    {
        var result = await api.GetMovie(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchMovieResultAction(result));
    }
}
