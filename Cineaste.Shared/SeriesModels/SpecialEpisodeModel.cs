namespace Cineaste.Shared.SeriesModels;

using System.Text.Json.Serialization;

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
    string? RottenTomatoesLink) : SeriesComponentModel(SequenceNumber)
{
    [JsonIgnore]
    public override string Title =>
        this.Titles.First().Name;

    [JsonIgnore]
    public override string Years =>
        this.Year.ToString();
}
