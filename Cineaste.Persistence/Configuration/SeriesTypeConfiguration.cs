namespace Cineaste.Persistence.Configuration;

internal sealed class SeriesTypeConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> series)
    {
        series.HasStronglyTypedId();

        series.HasTitles(s => s.Titles, "SeriesTitles");

        series.HasMany(s => s.Seasons)
            .WithOne();

        series.Navigation(s => s.Seasons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        series.HasMany(s => s.SpecialEpisodes)
            .WithOne();

        series.Navigation(s => s.SpecialEpisodes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        series.Property(s => s.WatchStatus)
            .HasConversion<string>();

        series.Property(s => s.ReleaseStatus)
            .HasConversion<string>();

        series.HasPoster(s => s.Poster);

        series.HasTags(s => s.Tags, "SeriesTags");

        series.Ignore(s => s.FranchiseItem);
    }
}
