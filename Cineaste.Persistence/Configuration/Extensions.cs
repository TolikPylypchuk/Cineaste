namespace Cineaste.Persistence.Configuration;

using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal static class Extensions
{
    internal const string ListId = "ListId";

    public static void HasStronglyTypedId<T>(this EntityTypeBuilder<T> builder)
        where T : Entity<T>
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion<IdConverter<T>>();
    }

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
                title.Property(t => t.Priority);
                title.Property(t => t.IsOriginal);

                title.HasCheckConstraint($"CH_{tableName}_NameNotEmpty", "Name <> ''");
                title.HasCheckConstraint($"CH_{tableName}_PriorityPositive", "Priority > 0");
            })
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    public static void HasPoster<T>(this EntityTypeBuilder<T> builder, Expression<Func<T, Poster?>> poster)
        where T : Entity<T>
    {
        builder.OwnsOne(poster, poster => poster.Property(p => p.RawData).HasColumnName(nameof(Poster)));

        builder.Navigation(poster)
            .AutoInclude(false);
    }

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
