namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record UpdateMovieAction(Guid Id, MovieRequest Request);

public sealed record UpdateMovieResultAction(ApiResult<MovieModel> Result) : ResultAction<MovieModel>(Result);

public static class UpdateMovieReducers
{
    [ReducerMethod(typeof(UpdateMovieAction))]
    public static MovieFormState ReduceUpdateMovieAction(MovieFormState state) =>
        state with { IsUpdatingMovie = true };

    [ReducerMethod]
    public static MovieFormState ReduceUpdateMovieResultAction(MovieFormState state, UpdateMovieResultAction action) =>
        action.Handle(
            onSuccess: movie => state with { IsUpdatingMovie = false, MovieModel = movie },
            onFailure: problem => state with { IsUpdatingMovie = false, UpdateMovieProblem = problem });
}

[AutoConstructor]
public sealed partial class UpdateMovieEffect
{
    private readonly IMovieApi api;

    [EffectMethod]
    public async Task HandleUpdateMovie(UpdateMovieAction action, IDispatcher dispatcher)
    {
        var result = await this.api.UpdateMovie(action.Id, action.Request).ToApiResultAsync();
        dispatcher.Dispatch(new UpdateMovieResultAction(result));
    }
}
