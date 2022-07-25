namespace Cineaste.Client.Store.Forms.MovieForm;

[FeatureState]
public sealed record MovieFormState : FormState<MovieModel>
{
    public ImmutableList<ListKindModel> AvailableKinds { get; init; } = ImmutableList.Create<ListKindModel>();
}
