using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Cineaste.Persistence.Configuration;

internal static class Extensions
{
    internal const string ListId = nameof(ListId);

    public static void HasStronglyTypedId<T>(this EntityTypeBuilder<T> builder)
        where T : Entity<T>
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion<IdConverter<T>>();
    }

    public static void HasPosterHash<T>(this EntityTypeBuilder<T> builder, Expression<Func<T, string?>> posterHash)
        where T : Entity<T> =>
        builder.Property(posterHash)
            .HasMaxLength(SHA256.HashSizeInBytes * 2)
            .IsFixedLength();

    public static void HasTitles<T>(
        this EntityTypeBuilder<T> builder,
        Expression<Func<T, IEnumerable<Title>?>> titles,
        string tableName)
        where T : Entity<T> =>
        builder.OwnsMany(
            titles,
            title =>
            {
                title.ToTable(tableName);
                title.Property(t => t.Name);
                title.Property(t => t.SequenceNumber);
                title.Property(t => t.IsOriginal);

                title.ToTable(t => t.HasCheckConstraint($"CH_{tableName}_NameNotEmpty", "Name <> ''"));
                title.ToTable(t =>
                    t.HasCheckConstraint($"CH_{tableName}_SequenceNumberPositive", "SequenceNumber > 0"));
            })
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    public static void HasTags<T>(
        this EntityTypeBuilder<T> builder,
        Expression<Func<T, IEnumerable<TagContainer>?>> tags,
        string tableName)
        where T : Entity<T> =>
        builder.OwnsMany(
            tags,
            tag =>
            {
                tag.ToTable(tableName);
                tag.HasOne(t => t.Tag)
                    .WithMany();
            })
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    public static void HasFranchiseItem<T>(
        this EntityTypeBuilder<T> builder,
        Expression<Func<T, FranchiseItem?>> item,
        Expression<Func<FranchiseItem, T?>> entity)
        where T : Entity<T>
    {
        const string franchiseItemId = "FranchiseItemId";

        builder.Property<Id<FranchiseItem>?>(franchiseItemId)
            .HasConversion<IdConverter<FranchiseItem>>();

        builder.HasOne(item)
            .WithOne(entity)
            .HasForeignKey<T>(franchiseItemId);
    }

    public static void HasListId<T>(this EntityTypeBuilder<T> builder)
        where T : Entity<T> =>
        builder.Property<Id<CineasteList>>(ListId)
            .HasConversion<IdConverter<CineasteList>>();

    public static void HasManyToOne<T>(
        this EntityTypeBuilder<CineasteList> list,
        Expression<Func<CineasteList, IEnumerable<T>?>> items)
        where T : Entity<T>
    {
        list.HasMany(items)
            .WithOne()
            .HasForeignKey(ListId);

        list.Navigation(items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
