namespace Cineaste.Client.Store.Forms.LimitedSeriesForm;

[FeatureState]
public sealed record LimitedSeriesFormState : PosterFormState<LimitedSeriesModel>
{
    public ApiCall Convert { get; init; } = ApiCall.NotStarted();
}
