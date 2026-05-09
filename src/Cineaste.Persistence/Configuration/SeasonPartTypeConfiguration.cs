namespace Cineaste.Persistence.Configuration;

internal sealed class SeasonPartTypeConfiguration : IEntityTypeConfiguration<SeasonPart>
{
    public void Configure(EntityTypeBuilder<SeasonPart> seasonPart)
    {
        seasonPart.HasKey(p => p.Id);

        seasonPart.HasReleasePeriod(p => p.Period);

        seasonPart.Property(p => p.PosterHash)
            .IsFixedLength();
    }
}
