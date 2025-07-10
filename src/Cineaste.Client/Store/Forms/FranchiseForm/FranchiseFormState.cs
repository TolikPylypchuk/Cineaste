namespace Cineaste.Client.Store.Forms.FranchiseForm;

[FeatureState]
public sealed record FranchiseFormState : FormState<FranchiseModel>
{
    public ImmutableList<ListKindModel> AvailableKinds { get; init; } = [];
}
