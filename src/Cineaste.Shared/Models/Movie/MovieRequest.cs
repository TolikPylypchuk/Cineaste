using Cineaste.Shared.Validation.Movie;

namespace Cineaste.Shared.Models.Movie;

public sealed record MovieRequest(
    Guid ListId,
    ImmutableValueList<TitleRequest> Titles,
    ImmutableValueList<TitleRequest> OriginalTitles,
    int Year,
    bool IsWatched,
    bool IsReleased,
    Guid KindId,
    string? ImdbId,
    string? RottenTomatoesId) : IValidatable<MovieRequest>, ITitledRequest
{
    public static IValidator<MovieRequest> Validator { get; } = new MovieRequestValidator();
}
