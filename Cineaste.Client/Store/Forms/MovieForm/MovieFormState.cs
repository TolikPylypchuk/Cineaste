namespace Cineaste.Client.Store.Forms.MovieForm;

[FeatureState]
public sealed record MovieFormState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }

    public MovieModel? MovieModel { get; init; }
    public ProblemDetails? FetchProblem { get; init; }

    public ImmutableList<ListKindModel> AvailableKinds { get; init; } = ImmutableList.Create<ListKindModel>();

    public bool IsCreatingMovie { get; init; }
    public ProblemDetails? CreateMovieProblem { get; init; }
}
