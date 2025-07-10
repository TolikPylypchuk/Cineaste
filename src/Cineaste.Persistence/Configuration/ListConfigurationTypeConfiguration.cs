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
                sorting.Property(s => s.DefaultFirstSortOrder)
                    .HasColumnName(nameof(ListSortingConfiguration.DefaultFirstSortOrder))
                    .HasConversion<string>();

                sorting.Property(s => s.DefaultFirstSortDirection)
                    .HasColumnName(nameof(ListSortingConfiguration.DefaultFirstSortDirection))
                    .HasConversion<string>();

                sorting.Property(s => s.DefaultSecondSortOrder)
                    .HasColumnName(nameof(ListSortingConfiguration.DefaultSecondSortOrder))
                    .HasConversion<string>();

                sorting.Property(s => s.DefaultSecondSortDirection)
                    .HasColumnName(nameof(ListSortingConfiguration.DefaultSecondSortDirection))
                    .HasConversion<string>();
            });
    }
}
