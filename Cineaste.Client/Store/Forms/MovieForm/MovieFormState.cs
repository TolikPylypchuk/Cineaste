namespace Cineaste.Client.Store.Forms.MovieForm;

[FeatureState]
public sealed record MovieFormState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }

    public MovieModel? MovieModel { get; init; }
    public ProblemDetails? FetchMovieProblem { get; init; }

    public ImmutableList<ListKindModel> AvailableKinds { get; init; } = ImmutableList.Create<ListKindModel>();

    public bool IsCreatingMovie { get; init; }
    public ProblemDetails? CreateMovieProblem { get; init; }

    public bool IsDeletingMovie { get; init; }
    public ProblemDetails? DeleteMovieProblem { get; init; }
}
