namespace Cineaste.Persistence.Configuration;

internal sealed class SeriesTypeConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> series)
    {
        series.HasStronglyTypedId();
        series.HasTitles(s => s.AllTitles, "SeriesTitles");
        series.HasPoster(s => s.Poster);

        series.HasMany(s => s.Seasons)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        series.Navigation(s => s.Seasons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        series.HasMany(s => s.SpecialEpisodes)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        series.Navigation(s => s.SpecialEpisodes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        series.Property(s => s.WatchStatus)
            .HasConversion<string>();

        series.Property(s => s.ReleaseStatus)
            .HasConversion<string>();

        series.HasOne(s => s.Kind)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        series.HasTags(s => s.Tags, "SeriesTags");
        series.HasFranchiseItem(s => s.FranchiseItem, fi => fi.Series);

        series.Ignore(s => s.Titles);
        series.Ignore(s => s.OriginalTitles);
        series.Ignore(s => s.Title);
        series.Ignore(s => s.OriginalTitle);
        series.Ignore(s => s.StartYear);
        series.Ignore(s => s.EndYear);
    }
}
