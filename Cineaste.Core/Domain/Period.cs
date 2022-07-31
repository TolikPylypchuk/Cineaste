namespace Cineaste.Core.Domain;

public sealed class Period : Entity<Period>
{
    private int startMonth;
    private int startYear;
    private int endMonth;
    private int endYear;
    private int episodeCount;
    private string? rottenTomatoesId;

    public int StartMonth
    {
        get => this.startMonth;
        set => this.startMonth = Require.Month(value);
    }

    public int StartYear
    {
        get => this.startYear;
        set => this.startYear = Require.Positive(value);
    }

    public int EndMonth
    {
        get => this.endMonth;
        set => this.endMonth = Require.Month(value);
    }

    public int EndYear
    {
        get => this.endYear;
        set => this.endYear = Require.Positive(value);
    }

    public bool IsSingleDayRelease { get; set; }

    public int EpisodeCount
    {
        get => this.episodeCount;
        set => this.episodeCount = Require.Positive(value);
    }

    public string? RottenTomatoesId
    {
        get => this.rottenTomatoesId;
        set => this.rottenTomatoesId = Require.RottenTomatoesId(value);
    }

    public Poster? Poster { get; set; }

    public Period(
        Id<Period> id,
        int startMonth,
        int startYear,
        int endMonth,
        int endYear,
        bool isSingleDayRelease,
        int episodeCount)
        : base(id)
    {
        this.StartMonth = startMonth;
        this.StartYear = startYear;
        this.EndMonth = endMonth;
        this.EndYear = endYear;
        this.IsSingleDayRelease = isSingleDayRelease;
        this.EpisodeCount = episodeCount;
    }
}
