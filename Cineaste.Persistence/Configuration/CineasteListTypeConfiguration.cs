namespace Cineaste.Persistence.Configuration;

internal sealed class CineasteListTypeConfiguration : IEntityTypeConfiguration<CineasteList>
{
    public void Configure(EntityTypeBuilder<CineasteList> list)
    {
        list.HasStronglyTypedId();

        list.HasIndex(l => l.Handle)
            .IsUnique();

        list.ToTable(t => t.HasCheckConstraint($"CH_Lists_NameNotEmpty", "Name <> ''"));
        list.ToTable(t => t.HasCheckConstraint($"CH_Lists_HandleNotEmpty", "Handle <> ''"));

        list.HasOne(l => l.Configuration)
            .WithOne()
            .HasForeignKey<ListConfiguration>(Extensions.ListId);

        list.HasManyToOne(l => l.Movies);
        list.HasManyToOne(l => l.Series);
        list.HasManyToOne(l => l.Franchises);
        list.HasManyToOne(l => l.MovieKinds);
        list.HasManyToOne(l => l.SeriesKinds);
        list.HasManyToOne(l => l.Tags);
    }
}
