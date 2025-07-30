namespace Cineaste.Client.Store.Forms.FranchiseForm;

[FeatureState]
public sealed record FranchiseFormState() : FormState<FranchiseModel>
{
    public Guid? InitialItemId { get; init; } = null;
}
