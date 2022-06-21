namespace Cineaste.Shared.MovieModels;

public sealed record MovieModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    int Year,
    SimpleKindModel Kind,
    string? ImdbId,
    string? RottenTomatoesLink);
