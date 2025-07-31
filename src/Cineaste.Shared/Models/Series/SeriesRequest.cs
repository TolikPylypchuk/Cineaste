using Cineaste.Shared.Validation.Series;

namespace Cineaste.Shared.Models.Series;

public sealed record SeriesRequest(
    ImmutableValueList<TitleRequest> Titles,
    ImmutableValueList<TitleRequest> OriginalTitles,
    SeriesWatchStatus WatchStatus,
    SeriesReleaseStatus ReleaseStatus,
    Guid KindId,
    ImmutableValueList<SeasonRequest> Seasons,
    ImmutableValueList<SpecialEpisodeRequest> SpecialEpisodes,
    string? ImdbId,
    string? RottenTomatoesId,
    Guid? ParentFranchiseId) : IValidatable<SeriesRequest>, ITitledRequest
{
    public static IValidator<SeriesRequest> Validator { get; } = new SeriesRequestValidator();
}
