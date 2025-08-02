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
    FranchiseItemInfoModel? FranchiseItem) : IIdentifyableModel, ITitledModel;

public record FranchiseItemModel(
    Guid Id,
    FranchiseItemType Type,
    int SequenceNumber,
    int? DisplayNumber,
    string Title,
    string OriginalTitle,
    int? StartYear,
    int? EndYear,
    string ListItemColor,
    int ListSequenceNumber) : IIdentifyableModel;
