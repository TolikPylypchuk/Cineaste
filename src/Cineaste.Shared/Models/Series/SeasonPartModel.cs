namespace Cineaste.Shared.Models.Series;

public sealed record SeasonPartModel(
    Guid Id,
    ReleasePeriodModel Period,
    string? RottenTomatoesId,
    string? PosterUrl) : IIdentifyableModel;
