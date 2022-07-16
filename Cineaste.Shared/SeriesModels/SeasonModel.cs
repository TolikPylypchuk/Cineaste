namespace Cineaste.Shared.SeriesModels;

using System.Text.Json.Serialization;

public sealed record SeasonModel(
    Guid Id,
    ImmutableList<TitleModel> Titles,
    ImmutableList<TitleModel> OriginalTitles,
    int SequenceNumber,
    SeasonWatchStatus WatchStatus,
    SeasonReleaseStatus ReleaseStatus,
    string Channel,
    ImmutableList<PeriodModel> Periods) : SeriesComponentModel(SequenceNumber)
{
    [JsonIgnore]
    public override string Title =>
        this.Titles.First().Name;

    [JsonIgnore]
    public override string Years
    {
        get
        {
            int startYear = this.Periods.Min(period => period.StartYear);
            int endYear = this.Periods.Max(period => period.EndYear);

            return startYear == endYear ? startYear.ToString() : $"{startYear}-{endYear}";
        }
    }
}
