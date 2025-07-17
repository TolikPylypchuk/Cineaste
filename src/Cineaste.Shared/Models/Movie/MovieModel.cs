namespace Cineaste.Shared.Models.Movie;

public sealed record MovieModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    int Year,
    bool IsWatched,
    bool IsReleased,
    ListKindModel Kind,
    string? ImdbId,
    string? RottenTomatoesId,
    string ListItemColor,
    Guid? ParentFranchiseId,
    int? SequenceNumber,
    string DisplayNumber,
    bool IsFirstInFranchise,
    bool IsLastInFranchise) : ITitledModel;
