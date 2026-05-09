namespace Cineaste.Core.Domain;

public readonly record struct ReleasePeriod
{
    public Month StartMonth { get; }
    public int StartYear { get; }
    public Month EndMonth { get; }
    public int EndYear { get; }

    public bool IsSingleDayRelease { get; init; }

    public int EpisodeCount { get; }

    public ReleasePeriod(
        Month startMonth,
        int startYear,
        Month endMonth,
        int endYear,
        bool isSingleDayRelease,
        int episodeCount)
    {
        this.StartMonth = startMonth;
        this.StartYear = Require.Positive(startYear);
        this.EndMonth = endMonth;
        this.EndYear = Require.Positive(endYear);
        this.IsSingleDayRelease = isSingleDayRelease;
        this.EpisodeCount = Require.Positive(episodeCount);
    }
}
