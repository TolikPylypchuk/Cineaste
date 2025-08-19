namespace Cineaste.Persistence.Configuration;

internal sealed class PeriodTypeConfiguration : IEntityTypeConfiguration<Period>
{
    public void Configure(EntityTypeBuilder<Period> period)
    {
        period.HasKey(p => p.Id);

        period.Property(p => p.PosterHash)
            .IsFixedLength();

        period.ToTable(t => t.HasCheckConstraint(
            "CH_Periods_StartMonthValid", "[StartMonth] >= 1 AND [StartMonth] <= 12"));

        period.ToTable(t => t.HasCheckConstraint("CH_Periods_StartYearPositive", "[StartYear] > 0"));

        period.ToTable(t => t.HasCheckConstraint(
            "CH_Periods_EndMonthValid", "[StartMonth] >= 1 AND [StartMonth] <= 12"));

        period.ToTable(t => t.HasCheckConstraint("CH_Periods_EndYearPositive", "[EndYear] > 0"));

        period.ToTable(t => t.HasCheckConstraint(
            "CH_Periods_PeriodValid",
            "DATEFROMPARTS([StartYear], [StartMonth], 1) <= DATEFROMPARTS([EndYear], [EndMonth], 1)"));

        period.ToTable(t => t.HasCheckConstraint("CH_Periods_EpisodeCountPositive", "[EpisodeCount] > 0"));
    }
}
