namespace Cineaste.Persistence.Configuration;

internal sealed class SeasonTypeConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> season)
    {
        season.HasStronglyTypedId();

        season.HasTitles(s => s.Titles, "SeasonTitles");

        season.HasCheckConstraint("CH_Seasons_ChannelNotEmpty", "Channel <> ''");
        season.HasCheckConstraint("CH_Seasons_SequenceNumberPositive", "SequenceNumber > 0");

        season.HasMany(s => s.Periods)
            .WithOne();

        season.Navigation(s => s.Periods)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        season.Property(s => s.WatchStatus)
            .HasConversion<string>();

        season.Property(s => s.ReleaseStatus)
            .HasConversion<string>();
    }
}
