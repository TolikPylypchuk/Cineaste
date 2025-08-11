namespace Cineaste.Client.Store.Forms.FranchiseForm;

[FeatureState]
public sealed record FranchiseFormState() : PosterFormState<FranchiseModel>
{
    public Guid? InitialItemId { get; init; } = null;

    public ApiCall FetchStandaloneItems { get; init; } = ApiCall.NotStarted();
    public ImmutableList<ListItemModel> StandaloneItems { get; init; } = [];
}
