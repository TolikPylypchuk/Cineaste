namespace Cineaste.Shared.Models.Series;

using Cineaste.Shared.Validation.Series;

public sealed record SeriesRequest(
    Guid ListId,
    ImmutableList<TitleRequest> Titles,
    ImmutableList<TitleRequest> OriginalTitles,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    Guid KindId,
    ImmutableList<SeasonRequest> Seasons,
    ImmutableList<SpecialEpisodeRequest> SpecialEpisodes,
    string? ImdbId,
    string? RottenTomatoesId) : IValidatable<SeriesRequest>, ITitledRequest
{
    public static IValidator<SeriesRequest> CreateValidator() =>
        new SeriesRequestValidator();
}
