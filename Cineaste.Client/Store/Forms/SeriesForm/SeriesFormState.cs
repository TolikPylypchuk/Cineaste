namespace Cineaste.Client.Store.Forms.SeriesForm;

[FeatureState]
public sealed record SeriesFormState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }

    public SeriesModel? SeriesModel { get; init; }
    public ProblemDetails? FetchSeriesProblem { get; init; }

    public ImmutableList<ListKindModel> AvailableKinds { get; init; } = ImmutableList.Create<ListKindModel>();

    public ImmutableArray<SeriesWatchStatus> AllWatchStatuses { get; } =
        Enum.GetValues<SeriesWatchStatus>().ToImmutableArray();

    public ImmutableArray<SeriesReleaseStatus> AllReleaseStatuses { get; } =
        Enum.GetValues<SeriesReleaseStatus>().ToImmutableArray();

    public bool IsCreatingSeries { get; init; }
    public ProblemDetails? CreateSeriesProblem { get; init; }

    public bool IsUpdatingSeries { get; init; }
    public ProblemDetails? UpdateSeriesProblem { get; init; }

    public bool IsDeletingSeries { get; init; }
    public ProblemDetails? DeleteSeriesProblem { get; init; }
}
