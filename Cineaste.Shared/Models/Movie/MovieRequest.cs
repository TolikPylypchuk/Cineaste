namespace Cineaste.Shared.Models.Movie;

public sealed record MovieRequest(
    Guid ListId,
    ImmutableList<TitleRequest> Titles,
    ImmutableList<TitleRequest> OriginalTitles,
    int Year,
    bool IsWatched,
    bool IsReleased,
    Guid KindId,
    string? ImdbId,
    string? RottenTomatoesId) : IValidatable<MovieRequest>
{
    public static IValidator<MovieRequest> CreateValidator() =>
        new MovieRequestValidator();
}
