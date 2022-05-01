namespace Cineaste.Core.Domain;

public sealed class FranchiseItem : Entity<FranchiseItem>
{
    private Franchise parentFranchise;

    public Franchise ParentFranchise
    {
        get => this.parentFranchise;

        [MemberNotNull(nameof(parentFranchise))]
        set => this.parentFranchise = Require.NotNull(value);
    }

    public int SequenceNumber { get; set; }
    public bool ShouldDisplayNumber { get; set; }

    public FranchiseItem(Id<FranchiseItem> id, Franchise parentFranchise, int sequenceNumber, bool shouldDisplayNumber)
        : base(id)
    {
        this.ParentFranchise = parentFranchise;
        this.SequenceNumber = sequenceNumber;
        this.ShouldDisplayNumber = shouldDisplayNumber;
    }
}
