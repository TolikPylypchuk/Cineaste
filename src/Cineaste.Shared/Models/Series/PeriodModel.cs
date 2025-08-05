namespace Cineaste.Shared.Models.Series;

public sealed record PeriodModel(
    Guid Id,
    int StartMonth,
    int StartYear,
    int EndMonth,
    int EndYear,
    int EpisodeCount,
    bool IsSingleDayRelease,
    string? RottenTomatoesId,
    string? PosterUrl) : IIdentifyableModel;
