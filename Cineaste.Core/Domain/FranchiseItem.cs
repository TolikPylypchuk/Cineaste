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
}
