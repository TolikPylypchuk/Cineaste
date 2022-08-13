namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record FetchMovieAction(Guid Id, ImmutableList<ListKindModel> AvailableKinds);

public sealed record FetchMovieResultAction(ApiResult<MovieModel> Result) : ResultAction<MovieModel>(Result);

public static class FetchMovieReducers
{
    [ReducerMethod]
    public static MovieFormState ReduceFetchMovieAction(MovieFormState _, FetchMovieAction action) =>
        new() { Fetch = ApiCall.InProgress(), AvailableKinds = action.AvailableKinds };

    [ReducerMethod]
    public static MovieFormState ReduceFetchMovieResultAction(MovieFormState state, FetchMovieResultAction action) =>
        action.Handle(
            onSuccess: movie => state with { Fetch = ApiCall.Success(), Model = movie },
            onFailure: problem => state with { Fetch = ApiCall.Failure(problem) });
}

[AutoConstructor]
public sealed partial class FetchMovieEffect
{
    private readonly IMovieApi api;

    [EffectMethod]
    public async Task HandleFetchMovie(FetchMovieAction action, IDispatcher dispatcher)
    {
        var result = await this.api.GetMovie(action.Id).ToApiResultAsync();
        dispatcher.Dispatch(new FetchMovieResultAction(result));
    }
}
