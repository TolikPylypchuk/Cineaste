namespace Cineaste.Shared.Models.Series;

public sealed record ReleasePeriodModel(
    int StartMonth,
    int StartYear,
    int EndMonth,
    int EndYear,
    int EpisodeCount,
    bool IsSingleDayRelease);
