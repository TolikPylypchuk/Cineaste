namespace Cineaste.Shared.ListModels;

using System.ComponentModel;

public sealed record SimpleListModel(Guid Id, string Name, string Handle);

public sealed record ListModel(
    Guid Id,
    string Name,
    string Handle,
    ListConfigurationModel Config,
    ImmutableList<ListItemModel> Movies,
    ImmutableList<ListItemModel> Series,
    ImmutableList<ListItemModel> Franchises,
    ImmutableList<ListKindModel> MovieKinds,
    ImmutableList<ListKindModel> SeriesKinds);

public sealed record ListConfigurationModel(
    string Culture,
    string DefaultSeasonTitle,
    string DefaultSeasonOriginalTitle,
    ListSortOrder DefaultFirstSortOrder,
    ListSortDirection DefaultFirstSortDirection,
    ListSortOrder DefaultSecondSortOrder,
    ListSortDirection DefaultSecondSortDirection);
