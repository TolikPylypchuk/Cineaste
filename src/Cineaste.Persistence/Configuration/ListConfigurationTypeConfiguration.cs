using System.Globalization;

namespace Cineaste.Persistence.Configuration;

internal sealed class ListConfigurationTypeConfiguration : IEntityTypeConfiguration<ListConfiguration>
{
    public void Configure(EntityTypeBuilder<ListConfiguration> config)
    {
        config.HasStronglyTypedId();
        config.HasListId();

        config.Property(c => c.Culture)
            .HasConversion(culture => culture.ToString(), code => CultureInfo.GetCultureInfo(code));

        config.OwnsOne(
            c => c.SortingConfiguration,
            sorting =>
            {
                sorting.Property(s => s.FirstSortOrder)
                    .HasColumnName(nameof(ListSortingConfiguration.FirstSortOrder))
                    .HasConversion<string>();

                sorting.Property(s => s.SecondSortOrder)
                    .HasColumnName(nameof(ListSortingConfiguration.SecondSortOrder))
                    .HasConversion<string>();

                sorting.Property(s => s.SortDirection)
                    .HasColumnName(nameof(ListSortingConfiguration.SortDirection))
                    .HasConversion<string>();
            });
    }
}
