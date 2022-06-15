namespace Cineaste.Client.Store.CreateListPage;

[FeatureState]
public sealed record CreateListPageState
{
    public bool IsCreatingList { get; init; }

    public ImmutableList<SimpleCultureModel> AllCultures { get; init; } = ImmutableList.Create<SimpleCultureModel>();

    public ProblemDetails? CulturesProblem { get; init; }
    public ProblemDetails? CreateListProblem { get; init; }
}
