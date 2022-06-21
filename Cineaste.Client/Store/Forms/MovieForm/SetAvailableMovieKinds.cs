namespace Cineaste.Client.Store.Forms.MovieForm;

public sealed record SetAvailableMovieKindsAction(ImmutableList<SimpleKindModel> AvailableKinds);

public static class SetAvailableMovieKindsReducers
{
    [ReducerMethod]
    public static MovieFormState ReduceSetAvailableMovieKinds(
        MovieFormState state,
        SetAvailableMovieKindsAction action) =>
        state with { AvailableKinds = action.AvailableKinds };
}
