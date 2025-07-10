namespace Cineaste.Persistence.Configuration;

internal sealed class FranchiseItemTypeConfiguration : IEntityTypeConfiguration<FranchiseItem>
{
    public void Configure(EntityTypeBuilder<FranchiseItem> item)
    {
        item.HasStronglyTypedId();
        item.ToTable(t => t.HasCheckConstraint("CH_FranchiseItems_SequenceNumberPositive", "SequenceNumber > 0"));

        item.Ignore(fi => fi.Titles);
        item.Ignore(fi => fi.Title);
        item.Ignore(fi => fi.OriginalTitle);
    }
}
