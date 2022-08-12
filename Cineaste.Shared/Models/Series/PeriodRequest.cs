namespace Cineaste.Shared.Models.Series;

public sealed record PeriodRequest(
    Guid? Id,
    int StartMonth,
    int StartYear,
    int EndMonth,
    int EndYear,
    int EpisodeCount,
    bool IsSingleDayRelease,
    string? RottenTomatoesId);
