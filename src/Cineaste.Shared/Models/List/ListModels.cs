using System.ComponentModel;

namespace Cineaste.Shared.Models.List;

public sealed record ListModel(
    Guid Id,
    ListConfigurationModel Config,
    ImmutableList<ListKindModel> MovieKinds,
    ImmutableList<ListKindModel> SeriesKinds);

public sealed record ListConfigurationModel(
    string Culture,
    string DefaultSeasonTitle,
    string DefaultSeasonOriginalTitle,
    ListSortOrder FirstSortOrder,
    ListSortOrder SecondSortOrder,
    ListSortDirection SortDirection);
