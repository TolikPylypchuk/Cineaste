namespace Cineaste.Persistence.Configuration;

internal sealed class SeasonTypeConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> season)
    {
        season.HasKey(s => s.Id);

        season.HasTitles("SeasonTitles");

        season.ToTable(t => t.HasCheckConstraint("CH_Seasons_ChannelNotEmpty", "[Channel] <> ''"));
        season.ToTable(t => t.HasCheckConstraint("CH_Seasons_SequenceNumberPositive", "[SequenceNumber] > 0"));

        season.HasMany(s => s.Parts)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        season.Navigation(s => s.Parts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        season.Ignore(s => s.StartYear);
        season.Ignore(s => s.EndYear);
    }
}
