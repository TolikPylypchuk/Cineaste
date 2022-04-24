namespace Cineaste.Core.Domain;

public sealed class FranchiseItem : DomainObject
{
    public int? MovieId { get; set; }

    public Movie? Movie { get; set; }

    public Series? Series { get; set; }

    public Franchise? Franchise { get; set; }

    public Franchise ParentFranchise { get; set; } = null!;

    public int SequenceNumber { get; set; }
    public int? DisplayNumber { get; set; }

    public List<Title> Titles =>
        (this.Movie, this.Series, this.Franchise) switch
        {
            (var movie, null, null) when movie != null => movie.Titles,
            (null, var series, null) when series != null => series.Titles,
            (null, null, var franchise) when franchise != null => franchise.ActualTitles,
            _ => throw new InvalidOperationException("Exactly one franchise entry component must be non-null")
        };

    public override string ToString() =>
        $"Franchise Entry #{this.Id}: {Title.ToString(this.Titles)}";
}
