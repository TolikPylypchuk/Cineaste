namespace Cineaste.Shared.Models.Series;

public sealed record SpecialEpisodeRequest(
    Guid? Id,
    ImmutableList<TitleRequest> Titles,
    ImmutableList<TitleRequest> OriginalTitles,
    int SequenceNumber,
    bool IsWatched,
    bool IsReleased,
    string Channel,
    int Month,
    int Year,
    string? RottenTomatoesId);
