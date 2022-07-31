namespace Cineaste.Client.Store.Forms.SeriesForm;

[FeatureState]
public sealed record SeriesFormState : FormState<SeriesModel>
{
    public ISeriesComponentFormModel? SelectedSeriesComponent { get; init; }

    public ImmutableList<ListKindModel> AvailableKinds { get; init; } = ImmutableList.Create<ListKindModel>();
}