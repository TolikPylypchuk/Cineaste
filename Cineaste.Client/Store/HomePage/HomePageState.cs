namespace Cineaste.Client.Store.HomePage;

[FeatureState]
public record HomePageState
{
    public bool IsLoading { get; init; }
    public IReadOnlyCollection<SimpleListModel> Lists { get; init; } = Array.Empty<SimpleListModel>();
    public ProblemDetails? Problem { get; init; }
}
