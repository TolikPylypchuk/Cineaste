namespace Cineaste.Client.Store.Forms.SeriesForm;

[FeatureState]
public sealed record SeriesFormState : FormState<SeriesModel>
{
    public ImmutableList<ListKindModel> AvailableKinds { get; init; } = ImmutableList.Create<ListKindModel>();

    public ImmutableArray<SeriesWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeriesWatchStatus>().ToImmutableArray();

    public ImmutableArray<SeriesReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeriesReleaseStatus>().ToImmutableArray();
}
