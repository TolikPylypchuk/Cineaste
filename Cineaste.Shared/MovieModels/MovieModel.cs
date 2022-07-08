namespace Cineaste.Shared.MovieModels;

public sealed record MovieModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    int Year,
    bool IsWatched,
    bool IsReleased,
    ListKindModel Kind,
    string? ImdbId,
    string? RottenTomatoesLink,
    string DisplayNumber);
