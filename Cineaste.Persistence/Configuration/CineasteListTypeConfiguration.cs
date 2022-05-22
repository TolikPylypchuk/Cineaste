namespace Cineaste.Persistence.Configuration;

internal sealed class CineasteListTypeConfiguration : IEntityTypeConfiguration<CineasteList>
{
    public void Configure(EntityTypeBuilder<CineasteList> list)
    {
        list.HasStronglyTypedId();

        list.HasCheckConstraint($"CH_Lists_NameNotEmpty", "Name <> ''");

        list.HasManyToOne(l => l.Movies);
        list.HasManyToOne(l => l.Series);
        list.HasManyToOne(l => l.Franchises);
        list.HasManyToOne(l => l.MovieKinds);
        list.HasManyToOne(l => l.SeriesKinds);
        list.HasManyToOne(l => l.Tags);

        list.HasData(new CineasteList[]
        {
            new(Id.Create<CineasteList>(), "Test List 1"),
            new(Id.Create<CineasteList>(), "Test List 2"),
            new(Id.Create<CineasteList>(), "Test List 3"),
            new(Id.Create<CineasteList>(), "Test List 4"),
            new(Id.Create<CineasteList>(), "Test List 5")
        });
    }
}
