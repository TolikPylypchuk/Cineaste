namespace Cineaste.Core.Domain;

public sealed class Period : Entity<Period>
{
    private string? rottenTomatoesLink;

    public int StartMonth { get; set; }
    public int StartYear { get; set; }

    public int EndMonth { get; set; }
    public int EndYear { get; set; }

    public bool IsSingleDayRelease { get; set; }

    public int NumberOfEpisodes { get; set; }

    public string? RottenTomatoesLink
    {
        get => this.rottenTomatoesLink;
        set => this.rottenTomatoesLink = Require.Url(value);
    }

    public Poster? Poster { get; set; }

    public Period(
        Id<Period> id,
        int startMonth,
        int startYear,
        int endMonth,
        int endYear,
        bool isSingleDayRelease,
        int numberOfEpisodes,
        string? rottenTomatoesLink,
        Poster? poster)
        : base(id)
    {
        this.StartMonth = startMonth;
        this.StartYear = startYear;
        this.EndMonth = endMonth;
        this.EndYear = endYear;
        this.IsSingleDayRelease = isSingleDayRelease;
        this.NumberOfEpisodes = numberOfEpisodes;
        this.RottenTomatoesLink = rottenTomatoesLink;
        this.Poster = poster;
    }
}
