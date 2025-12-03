using System.Linq.Expressions;

namespace Cineaste.Persistence.Configuration;

internal static class Extensions
{
    internal const string ListId = nameof(ListId);

    extension<T>(EntityTypeBuilder<T> builder) where T : Entity<T>
    {
        public void HasTitles(Expression<Func<T, IEnumerable<Title>?>> titles, string tableName) =>
            builder.OwnsMany(
                titles,
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
