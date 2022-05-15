namespace Cineaste.Core.Domain;

public sealed class FranchiseItem : Entity<FranchiseItem>
{
    private Franchise parentFranchise;
    private int sequenceNumber;

    public Franchise ParentFranchise
    {
        get => this.parentFranchise;

        [MemberNotNull(nameof(parentFranchise))]
        set => this.parentFranchise = Require.NotNull(value);
    }

    public int SequenceNumber
    {
        get => this.sequenceNumber;
        set => this.sequenceNumber = Require.Positive(value);
    }

    public bool ShouldDisplayNumber { get; set; }

    public FranchiseItem(Id<FranchiseItem> id, Franchise parentFranchise, int sequenceNumber, bool shouldDisplayNumber)
        : base(id)
    {
        this.ParentFranchise = parentFranchise;
        this.SequenceNumber = sequenceNumber;
        this.ShouldDisplayNumber = shouldDisplayNumber;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "EF Core")]
    private FranchiseItem(Id<FranchiseItem> id)
        : base(id) =>
        this.parentFranchise = null!;

}
