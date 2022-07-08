namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record CreateMovieAction(MovieRequest Request);

public sealed record CreateMovieResultAction(ApiResult<MovieModel> Result) : ResultAction<MovieModel>(Result);

public static class CreateMovieReducers
{
    [ReducerMethod(typeof(CreateMovieAction))]
    public static MovieFormState ReduceCreateMovieAction(MovieFormState state) =>
        state with { IsCreatingMovie = true };

    [ReducerMethod]
    public static MovieFormState ReduceCreateMovieResultAction(MovieFormState state, CreateMovieResultAction action) =>
        action.Handle(
            onSuccess: movie => state with { IsCreatingMovie = false, MovieModel = movie },
            onFailure: problem => state with { IsCreatingMovie = false, CreateMovieProblem = problem });
}

[AutoConstructor]
public sealed partial class CreateMovieEffect
{
    private readonly IMovieApi api;

    [EffectMethod]
    public async Task HandleCreateMovie(CreateMovieAction action, IDispatcher dispatcher)
    {
        var result = await this.api.CreateMovie(action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new CreateMovieResultAction(result));
    }
}
