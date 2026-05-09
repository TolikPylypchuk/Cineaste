namespace Cineaste.Core.Domain;

public sealed class SeasonPart : Entity<SeasonPart>
{
    public ReleasePeriod Period { get; set; }

    public RottenTomatoesId? RottenTomatoesId { get; set; }

    public PosterHash? PosterHash { get; set; }

    public SeasonPart(Id<SeasonPart> id, ReleasePeriod period)
        : base(id) =>
        this.Period = period;

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private SeasonPart(Id<SeasonPart> id)
        : base(id)
    { }
}
