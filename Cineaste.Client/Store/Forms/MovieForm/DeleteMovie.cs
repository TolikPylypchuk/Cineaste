namespace Cineaste.Client.Store.Forms.MovieForm;

public record DeleteMovieAction(Guid Id);

public record DeleteMovieResultAction(Guid Id, EmptyApiResult Result) : EmptyResultAction(Result);

public static class DeleteMovieReducers
{
    [ReducerMethod(typeof(DeleteMovieAction))]
    public static MovieFormState ReduceDeleteMovieAction(MovieFormState state) =>
        state with { Delete = ApiCall.InProgress() };

    [ReducerMethod]
    public static MovieFormState ReduceDeleteMovieResultAction(MovieFormState state, DeleteMovieResultAction action) =>
        action.Handle(
            onSuccess: () => new MovieFormState(),
            onFailure: problem => state with { Delete = ApiCall.Failure(problem) });
}

[AutoConstructor]
public sealed partial class DeleteMovieEffect
{
    private readonly IMovieApi api;

    [EffectMethod]
    public async Task HandleDeleteMovieAction(DeleteMovieAction action, IDispatcher dispatcher)
    {
        var result = await this.api.DeleteMovie(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new DeleteMovieResultAction(action.Id, result));
    }
}
