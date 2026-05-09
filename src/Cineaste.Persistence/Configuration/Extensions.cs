using System.Linq.Expressions;

namespace Cineaste.Persistence.Configuration;

internal static class Extensions
{
    internal const string ListId = nameof(ListId);

    extension<T>(EntityTypeBuilder<T> builder) where T : Entity<T>
    {
        public void HasTags(Expression<Func<T, IEnumerable<TagContainer>?>> tags, string tableName) =>
            builder.OwnsMany(
                tags,
                tag =>
                {
                    tag.ToTable(tableName);
                    tag.HasOne(t => t.Tag)
                        .WithMany();
                })
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        public void HasFranchiseItem(
            Expression<Func<T, FranchiseItem?>> item,
            Expression<Func<FranchiseItem, T?>> entity)
        {
            const string franchiseItemId = "FranchiseItemId";

            builder.HasOne(item)
                .WithOne(entity)
                .HasForeignKey<T>(franchiseItemId);
        }

        public void HasListId() =>
            builder.Property<Id<CineasteList>>(ListId);

        public void HasReleasePeriod(Expression<Func<T, ReleasePeriod>> periodProperty)
        {
            builder.ComplexProperty(periodProperty, p =>
            {
                p.Property(p => p.StartMonth).HasColumnName(nameof(ReleasePeriod.StartMonth));
                p.Property(p => p.StartYear).HasColumnName(nameof(ReleasePeriod.StartYear));
                p.Property(p => p.EndMonth).HasColumnName(nameof(ReleasePeriod.EndMonth));
                p.Property(p => p.EndYear).HasColumnName(nameof(ReleasePeriod.EndYear));
                p.Property(p => p.IsSingleDayRelease).HasColumnName(nameof(ReleasePeriod.IsSingleDayRelease));
                p.Property(p => p.EpisodeCount).HasColumnName(nameof(ReleasePeriod.EpisodeCount));
            });

            builder.ToTable(t => t.HasCheckConstraint(
                $"CH_{t.Name}_StartMonthValid", "[StartMonth] >= 1 AND [StartMonth] <= 12"));

            builder.ToTable(t => t.HasCheckConstraint($"CH_{t.Name}_StartYearPositive", "[StartYear] > 0"));

            builder.ToTable(t => t.HasCheckConstraint(
                $"CH_{t.Name}_EndMonthValid", "[StartMonth] >= 1 AND [StartMonth] <= 12"));

            builder.ToTable(t => t.HasCheckConstraint($"CH_{t.Name}_EndYearPositive", "[EndYear] > 0"));

            builder.ToTable(t => t.HasCheckConstraint(
                $"CH_{t.Name}_PeriodValid",
                "DATEFROMPARTS([StartYear], [StartMonth], 1) <= DATEFROMPARTS([EndYear], [EndMonth], 1)"));

            builder.ToTable(t => t.HasCheckConstraint($"CH_{t.Name}_EpisodeCountPositive", "[EpisodeCount] > 0"));
        }
    }

    extension<T>(EntityTypeBuilder<T> builder) where T : TitledEntity<T>
    {
        public void HasTitles(string tableName)
        {
            builder.OwnsMany(
                e => e.AllTitles,
                title =>
                {
                    title.ToTable(tableName);
                    title.Property(t => t.Name);
                    title.Property(t => t.SequenceNumber);
                    title.Property(t => t.IsOriginal);

                    title.ToTable(t => t.HasCheckConstraint($"CH_{tableName}_NameNotEmpty", "[Name] <> ''"));
                    title.ToTable(t =>
                        t.HasCheckConstraint($"CH_{tableName}_SequenceNumberPositive", "[SequenceNumber] > 0"));
                })
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(e => e.Titles);
            builder.Ignore(e => e.OriginalTitles);
            builder.Ignore(e => e.Title);
            builder.Ignore(e => e.OriginalTitle);
        }
    }

    extension(EntityTypeBuilder<CineasteList> list)
    {
        public void HasManyToOne<T>(Expression<Func<CineasteList, IEnumerable<T>?>> items)
            where T : Entity<T>
        {
            list.HasMany(items)
                .WithOne()
                .HasForeignKey(ListId);

            list.Navigation(items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
