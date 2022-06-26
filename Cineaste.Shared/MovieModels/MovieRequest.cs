namespace Cineaste.Shared.MovieModels;

public sealed record MovieRequest(
    Guid ListId,
    ImmutableList<TitleRequest> Titles,
    ImmutableList<TitleRequest> OriginalTitles,
    int Year,
    bool IsWatched,
    bool IsReleased,
    Guid KindId,
    string? ImdbId,
    string? RottenTomatoesLink) : IValidatable<MovieRequest>
{
    public static IValidator<MovieRequest> CreateValidator() =>
        new MovieRequestValidator();
}
