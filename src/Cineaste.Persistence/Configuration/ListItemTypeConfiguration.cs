namespace Cineaste.Persistence.Configuration;

internal sealed class ListItemTypeConfiguration : IEntityTypeConfiguration<ListItem>
{
    public void Configure(EntityTypeBuilder<ListItem> item)
    {
        item.HasStronglyTypedId();

        item.ToTable(t => t.HasCheckConstraint("CH_ListItems_SequenceNumberPositive", "SequenceNumber > 0"));

        item.HasOne(i => i.List)
            .WithMany(l => l.Items)
            .HasForeignKey(Extensions.ListId);

        item.HasOne(i => i.Movie)
            .WithOne(m => m.ListItem)
            .HasForeignKey<ListItem>("MovieId");

        item.HasOne(i => i.Series)
            .WithOne(s => s.ListItem)
            .HasForeignKey<ListItem>("SeriesId"); ;

        item.HasOne(i => i.Franchise)
            .WithOne(f => f.ListItem)
            .HasForeignKey<ListItem>("FranchiseId");

        item.HasIndex(i => i.SequenceNumber);
    }
}
