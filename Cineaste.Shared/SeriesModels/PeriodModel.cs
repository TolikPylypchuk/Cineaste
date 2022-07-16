namespace Cineaste.Shared.SeriesModels;

public sealed record PeriodModel(
    Guid Id,
    int StartMonth,
    int StartYear,
    int EndMonth,
    int EndYear,
    int EpisodeCount,
    bool IsSingleDayRelease,
    string? RottenTomatoesLink);
