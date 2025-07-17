namespace Cineaste.Client.Store.ListPage;

public enum ListPageSelectionMode { None, Movie, Series, Franchise }

[FeatureState]
public sealed record ListPageState
{
    public bool IsLoading { get; init; }
    public bool IsLoaded { get; init; }

    public Guid Id { get; init; }

    public ImmutableList<ListItemModel> Items { get; init; } = [];

    public int Offset { get; init; }
    public int Size { get; init; }
    public int TotalItems { get; init; }

    public ImmutableList<ListKindModel> AvailableMovieKinds { get; init; } = [];
    public ImmutableList<ListKindModel> AvailableSeriesKinds { get; init; } = [];

    public ListConfigurationModel ListConfiguration { get; init; } = null!;

    public ListItemModel? SelectedItem { get; init; }
    public ListPageSelectionMode SelectionMode { get; init; }

    public ProblemDetails? Problem { get; init; }
}
