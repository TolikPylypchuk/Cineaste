namespace Cineaste.Shared.Models.Franchise;

public record FranchiseModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    ImmutableList<FranchiseItemModel> Items,
    ListKindModel Kind,
    FranchiseKindSource KindSource,
    bool ShowTitles,
    bool IsLooselyConnected,
    bool ContinueNumbering,
    string ListItemColor,
    int ListSequenceNumber,
    FranchiseItemInfoModel? FranchiseItem) : ITitledModel;

public record FranchiseItemModel(
    Guid Id,
    int SequenceNumber,
    int? DisplayNumber,
    string Title,
    int StartYear,
    int EndYear,
    FranchiseItemType Type);
