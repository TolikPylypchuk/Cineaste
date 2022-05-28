namespace Cineaste.Persistence.Configuration;

internal sealed class SpecialEpisodeTypeConfiguration : IEntityTypeConfiguration<SpecialEpisode>
{
    public void Configure(EntityTypeBuilder<SpecialEpisode> episode)
    {
        episode.HasStronglyTypedId();
        episode.HasTitles(e => e.Titles, "SpecialEpisodeTitles");

        episode.HasCheckConstraint("CH_SpecialEpisodes_MonthValid", "Month >= 1 AND Month <= 12");
        episode.HasCheckConstraint("CH_SpecialEpisodes_YearPositive", "Year > 0");

        episode.HasCheckConstraint("CH_SpecialEpisodes_ChannelNotEmpty", "Channel <> ''");
        episode.HasCheckConstraint("CH_SpecialEpisodes_SequenceNumberPositive", "SequenceNumber > 0");

        episode.HasPoster(e => e.Poster);

        episode.Ignore(e => e.Title);
        episode.Ignore(e => e.OriginalTitle);
    }
}
