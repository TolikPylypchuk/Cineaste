namespace Cineaste.Client.Store.ListPage;

[FeatureState]
public sealed record ListPageState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }

    public string Name { get; init; } = String.Empty;

    public ImmutableSortedSet<ListItemModel> Items { get; init; } = ImmutableSortedSet.Create<ListItemModel>();

    public ImmutableList<SimpleKindModel> AvailableMovieKinds { get; init; } = ImmutableList.Create<SimpleKindModel>();
    public ImmutableList<SimpleKindModel> AvailableSeriesKinds { get; init; } = ImmutableList.Create<SimpleKindModel>();

    public ListItemModel? SelectedItem { get; init; }

    public ProblemDetails? Problem { get; init; }
}
