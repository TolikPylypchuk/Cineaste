namespace Cineaste.Persistence.Configuration;

internal sealed class LimitedSeriesTypeConfiguration : IEntityTypeConfiguration<LimitedSeries>
{
    public void Configure(EntityTypeBuilder<LimitedSeries> limitedSeries)
    {
        limitedSeries.HasKey(ls => ls.Id);

        limitedSeries.HasTitles("LimitedSeriesTitles");

        limitedSeries.HasReleasePeriod(ls => ls.Period);

        limitedSeries.Property(ls => ls.PosterHash)
            .IsFixedLength();

        limitedSeries.HasTags(ls => ls.Tags, "LimitedSeriesTags");
        limitedSeries.HasFranchiseItem(ls => ls.FranchiseItem, fi => fi.LimitedSeries);
    }
}
