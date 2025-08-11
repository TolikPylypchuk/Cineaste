namespace Cineaste.Client.Store.Forms.SeriesForm;

[FeatureState]
public sealed record SeriesFormState : PosterFormState<SeriesModel>
{
    public ISeriesComponentFormModel? SelectedSeriesComponent { get; init; }
}
