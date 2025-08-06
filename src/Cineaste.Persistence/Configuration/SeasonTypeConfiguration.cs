namespace Cineaste.Persistence.Configuration;

internal sealed class SeasonTypeConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> season)
    {
        season.HasStronglyTypedId();

        season.HasTitles(s => s.AllTitles, "SeasonTitles");

        season.ToTable(t => t.HasCheckConstraint("CH_Seasons_ChannelNotEmpty", "Channel <> ''"));
        season.ToTable(t => t.HasCheckConstraint("CH_Seasons_SequenceNumberPositive", "SequenceNumber > 0"));

        season.HasMany(s => s.Periods)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        season.Navigation(s => s.Periods)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        season.Property(s => s.WatchStatus)
            .HasConversion<string>();

        season.Property(s => s.ReleaseStatus)
            .HasConversion<string>();

        season.Ignore(s => s.Titles);
        season.Ignore(s => s.OriginalTitles);
        season.Ignore(s => s.Title);
        season.Ignore(s => s.OriginalTitle);
        season.Ignore(s => s.StartYear);
        season.Ignore(s => s.EndYear);
    }
}
