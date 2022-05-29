namespace Cineaste.Shared;

using System.ComponentModel;

public sealed record SimpleListModel(Guid Id, string Name);

public sealed record ListModel(
    Guid Id,
    string Name,
    ListConfigurationModel Config,
    List<ListItemModel> Movies,
    List<ListItemModel> Series,
    List<ListItemModel> Franchises,
    List<ListKindModel> MovieKinds,
    List<ListKindModel> SeriesKinds);

public sealed record ListItemModel(
    Guid Id,
    ListItemType Type,
    bool ShouldBeShown,
    string DisplayNumber,
    string Title,
    string OriginalTitle,
    int StartYear,
    int EndYear,
    string Color,
    ListFranchiseItemModel? FranchiseItem);

public enum ListItemType { Movie, Series, Franchise }

public sealed record ListFranchiseItemModel(Guid FranchiseId, int SequenceNumber);

public sealed record ListKindModel(
    Guid Id,
    string Name,
    string WatchedColor,
    string NotWatchedColor,
    string NotReleasedColor,
    ListKindTarget Target);

public enum ListKindTarget { Movie, Series }

public sealed record ListConfigurationModel(
    string Culture,
    string DefaultSeasonTitle,
    string DefaultSeasonOriginalTitle,
    ListSortOrder DefaultFirstSortOrder,
    ListSortDirection DefaultFirstSortDirection,
    ListSortOrder DefaultSecondSortOrder,
    ListSortDirection DefaultSecondSortDirection);
