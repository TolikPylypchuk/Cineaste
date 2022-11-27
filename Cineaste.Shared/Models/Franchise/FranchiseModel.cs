namespace Cineaste.Shared.Models.Franchise;

public record FranchiseModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    ImmutableList<FranchiseItemModel> Items,
    bool ShowTitles,
    bool IsLooselyConnected,
    bool ContinueNumbering) : ITitledModel;

public record FranchiseItemModel(
    Guid Id,
    int SequenceNumber,
    string Title,
    int StartYear,
    int EndYear,
    FranchiseItemType Type);

public enum FranchiseItemType { Movie, Series, Franchise }
