namespace Cineaste.Persistence.Configuration;

internal sealed class SeasonTypeConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> season)
    {
        season.HasKey(s => s.Id);

        season.HasTitles(s => s.AllTitles, "SeasonTitles");

        season.ToTable(t => t.HasCheckConstraint("CH_Seasons_ChannelNotEmpty", "[Channel] <> ''"));
        season.ToTable(t => t.HasCheckConstraint("CH_Seasons_SequenceNumberPositive", "[SequenceNumber] > 0"));

        season.HasMany(s => s.Periods)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        season.Navigation(s => s.Periods)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        season.Ignore(s => s.Titles);
        season.Ignore(s => s.OriginalTitles);
        season.Ignore(s => s.Title);
        season.Ignore(s => s.OriginalTitle);
        season.Ignore(s => s.StartYear);
        season.Ignore(s => s.EndYear);
    }
}
