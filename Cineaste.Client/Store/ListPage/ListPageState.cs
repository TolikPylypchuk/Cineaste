namespace Cineaste.Client.Store.ListPage;

[FeatureState]
public sealed class ListPageState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }

    public string Name { get; init; } = String.Empty;
    public ImmutableSortedSet<ListItemModel> Items { get; init; } = ImmutableSortedSet.Create<ListItemModel>();
    public ListItemModel? SelectedItem { get; init; }

    public ProblemDetails? Problem { get; init; }
}
