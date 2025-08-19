namespace Cineaste.Persistence.Configuration;

internal sealed class CineasteListTypeConfiguration : IEntityTypeConfiguration<CineasteList>
{
    public void Configure(EntityTypeBuilder<CineasteList> list)
    {
        list.HasKey(l => l.Id);

        list.HasOne(l => l.Configuration)
            .WithOne()
            .HasForeignKey<ListConfiguration>(Extensions.ListId);

        list.HasManyToOne(l => l.MovieKinds);
        list.HasManyToOne(l => l.SeriesKinds);
        list.HasManyToOne(l => l.Tags);

        list.Navigation(l => l.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
