namespace Cineaste.Shared.Models.Series;

public sealed record SpecialEpisodeModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    int SequenceNumber,
    bool IsWatched,
    bool IsReleased,
    string Channel,
    int Month,
    int Year,
    string? RottenTomatoesId) : ISeriesComponentModel;
