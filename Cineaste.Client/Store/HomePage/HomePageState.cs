namespace Cineaste.Client.Store.HomePage;

[FeatureState]
public record HomePageState
{
    public bool IsLoading { get; init; }
    public ImmutableList<SimpleListModel> Lists { get; init; } = ImmutableList.Create<SimpleListModel>();
    public ProblemDetails? Problem { get; init; }
}
