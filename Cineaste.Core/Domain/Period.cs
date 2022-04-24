namespace Cineaste.Core.Domain;

public sealed class Period : DomainObject
{
    public int StartMonth { get; set; }
    public int StartYear { get; set; }

    public int EndMonth { get; set; }
    public int EndYear { get; set; }

    public bool IsSingleDayRelease { get; set; }

    public int NumberOfEpisodes { get; set; }

    public string? RottenTomatoesLink { get; set; }

    public string? PosterUrl { get; set; }

    public int SeasonId { get; set; }

    public Season Season { get; set; } = null!;

    public override string ToString()
    {
        string content = this.IsSingleDayRelease
            ? $"{this.StartMonth}.{this.StartYear}"
            : this.StartYear == this.EndYear
                ? this.StartMonth == this.EndMonth
                    ? $"{this.StartMonth}.{this.StartYear}"
                    : $"{this.StartMonth}-{this.EndMonth}.{this.StartYear}"
                : $"{this.StartMonth}.{this.StartYear}-{this.EndMonth}.{this.EndYear}";

        return $"Period #{this.Id}: {content}";
    }
}
