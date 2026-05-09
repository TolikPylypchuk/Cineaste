namespace Cineaste.Persistence.Configuration;

internal sealed class SeriesTypeConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> series)
    {
        series.HasKey(s => s.Id);
        series.HasTitles("SeriesTitles");

        series.HasMany(s => s.Seasons)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        series.Navigation(s => s.Seasons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        series.HasMany(s => s.SpecialEpisodes)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        series.Navigation(s => s.SpecialEpisodes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        series.HasOne(s => s.Kind)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        series.Property(s => s.PosterHash)
            .IsFixedLength();

        series.HasTags(s => s.Tags, "SeriesTags");
        series.HasFranchiseItem(s => s.FranchiseItem, fi => fi.Series);

        series.Ignore(s => s.StartYear);
        series.Ignore(s => s.EndYear);
    }
}
