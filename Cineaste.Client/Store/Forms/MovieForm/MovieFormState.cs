namespace Cineaste.Client.Store.Forms.MovieForm;

[FeatureState]
public sealed record MovieFormState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }
    public MovieModel? MovieModel { get; init; }
    public ProblemDetails? Problem { get; init; }
}
