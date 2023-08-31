namespace Cineaste.Client.Store.ListPage;

public enum ListPageSelectionMode { None, Movie, Series, Franchise }

[FeatureState]
public sealed record ListPageState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }

    public Guid Id { get; init; }

    public ListItemContainer Container { get; init; } = ListItemContainer.Empty;

    public ImmutableList<ListKindModel> AvailableMovieKinds { get; init; } = ImmutableList.Create<ListKindModel>();
    public ImmutableList<ListKindModel> AvailableSeriesKinds { get; init; } = ImmutableList.Create<ListKindModel>();

    public ListConfigurationModel ListConfiguration { get; init; } = null!;

    public ListItemModel? SelectedItem { get; init; }
    public ListPageSelectionMode SelectionMode { get; init; }

    public ProblemDetails? Problem { get; init; }
}
