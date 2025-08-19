namespace Cineaste.Persistence.Configuration;

internal sealed class SpecialEpisodeTypeConfiguration : IEntityTypeConfiguration<SpecialEpisode>
{
    public void Configure(EntityTypeBuilder<SpecialEpisode> episode)
    {
        episode.HasKey(e => e.Id);
        episode.HasTitles(e => e.AllTitles, "SpecialEpisodeTitles");

        episode.Property(e => e.PosterHash)
            .IsFixedLength();

        episode.ToTable(t => t.HasCheckConstraint("CH_SpecialEpisodes_MonthValid", "[Month] >= 1 AND [Month] <= 12"));
        episode.ToTable(t => t.HasCheckConstraint("CH_SpecialEpisodes_YearPositive", "[Year] > 0"));

        episode.ToTable(t => t.HasCheckConstraint("CH_SpecialEpisodes_ChannelNotEmpty", "[Channel] <> ''"));
        episode.ToTable(t => t.HasCheckConstraint("CH_SpecialEpisodes_SequenceNumberPositive", "[SequenceNumber] > 0"));

        episode.Ignore(e => e.Titles);
        episode.Ignore(e => e.OriginalTitles);
        episode.Ignore(e => e.Title);
        episode.Ignore(e => e.OriginalTitle);
    }
}
