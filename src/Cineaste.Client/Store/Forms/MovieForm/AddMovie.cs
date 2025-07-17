namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record AddMovieAction(MovieRequest Request);

public sealed record AddMovieResultAction(ApiResult<MovieModel> Result) : ResultAction<MovieModel>(Result);

public static class AddMovieReducers
{
    [ReducerMethod(typeof(AddMovieAction))]
    public static MovieFormState ReduceAddMovieAction(MovieFormState state) =>
        state with { Add = ApiCall.InProgress() };

    [ReducerMethod]
    public static MovieFormState ReduceAddMovieResultAction(MovieFormState state, AddMovieResultAction action) =>
        action.Handle(
            onSuccess: movie => state with { Add = ApiCall.Success(), Model = movie },
            onFailure: problem => state with { Add = ApiCall.Failure(problem) });
}

public sealed class AddMovieEffect(IMovieApi api)
{
    [EffectMethod]
    public async Task HandleAddMovie(AddMovieAction action, IDispatcher dispatcher)
    {
        var result = await api.AddMovie(action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new AddMovieResultAction(result));
    }
}
