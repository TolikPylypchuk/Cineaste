namespace Cineaste.Persistence.Configuration;

internal sealed class FranchiseItemTypeConfiguration : IEntityTypeConfiguration<FranchiseItem>
{
    public void Configure(EntityTypeBuilder<FranchiseItem> item)
    {
        item.HasStronglyTypedId();
        item.HasCheckConstraint("CH_FranchiseItems_SequenceNumberPositive", "SequenceNumber > 0");
    }
}
