namespace Cineaste.Shared.Models.List;

using System.ComponentModel;

public sealed record ListModel(
    Guid Id,
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
