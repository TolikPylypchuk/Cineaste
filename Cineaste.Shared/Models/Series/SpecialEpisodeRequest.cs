using Cineaste.Shared.Validation.Series;

namespace Cineaste.Shared.Models.Series;

public sealed record SpecialEpisodeRequest(
    Guid? Id,
    ImmutableValueList<TitleRequest> Titles,
    ImmutableValueList<TitleRequest> OriginalTitles,
    int SequenceNumber,
    bool IsWatched,
    bool IsReleased,
    string Channel,
    int Month,
    int Year,
    string? RottenTomatoesId) : IValidatable<SpecialEpisodeRequest>, ITitledRequest
{
    public static IValidator<SpecialEpisodeRequest> Validator { get; } = new SpecialEpisodeRequestValidator();
}
