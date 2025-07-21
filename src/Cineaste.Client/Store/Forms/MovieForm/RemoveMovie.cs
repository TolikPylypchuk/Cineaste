namespace Cineaste.Client.Store.Forms.MovieForm;

public record RemoveMovieAction(Guid Id);

public record RemoveMovieResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class RemoveMovieReducers
{
    [ReducerMethod(typeof(RemoveMovieAction))]
    public static MovieFormState ReduceRemoveMovieAction(MovieFormState state) =>
        state with { Remove = ApiCall.InProgress() };

    [ReducerMethod]
    public static MovieFormState ReduceRemoveMovieResultAction(MovieFormState state, RemoveMovieResultAction action) =>
        action.Handle(
            onSuccess: () => new MovieFormState(),
            onFailure: problem => state with { Remove = ApiCall.Failure(problem) });
}

public sealed class RemoveMovieEffects(IMovieApi api)
{
    [EffectMethod]
    public async Task HandleRemoveMovieAction(RemoveMovieAction action, IDispatcher dispatcher)
    {
        var result = await api.RemoveMovie(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new RemoveMovieResultAction(action.Id, result));
    }
}
