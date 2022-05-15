namespace Cineaste.Persistence.Configuration;

internal sealed class SeriesTypeConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> series)
    {
        series.HasStronglyTypedId();
        series.HasListId();
        series.HasTitles(s => s.Titles, "SeriesTitles");
        series.HasPoster(s => s.Poster);

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

        series.HasTags(s => s.Tags, "SeriesTags");
        series.HasFranchiseItem(s => s.FranchiseItem);
    }
}
