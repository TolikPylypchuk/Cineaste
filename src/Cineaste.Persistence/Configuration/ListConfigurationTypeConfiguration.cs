using System.Globalization;

namespace Cineaste.Persistence.Configuration;

internal sealed class ListConfigurationTypeConfiguration : IEntityTypeConfiguration<ListConfiguration>
{
    public void Configure(EntityTypeBuilder<ListConfiguration> config)
    {
        config.HasKey(c => c.Id);

        config.Property(c => c.Culture)
            .HasConversion(culture => culture.ToString(), code => CultureInfo.GetCultureInfo(code));

        config.OwnsOne(
            c => c.SortingConfiguration,
            sorting =>
            {
                sorting.Property(s => s.FirstSortOrder)
                    .HasColumnName(nameof(ListSortingConfiguration.FirstSortOrder));

                sorting.Property(s => s.SecondSortOrder)
                    .HasColumnName(nameof(ListSortingConfiguration.SecondSortOrder));

                sorting.Property(s => s.SortDirection)
                    .HasColumnName(nameof(ListSortingConfiguration.SortDirection));
            });
    }
}
