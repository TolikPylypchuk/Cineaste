namespace Cineaste.Persistence.Configuration;

internal sealed class KindTypeConfiguration<TKind>(string tableName) : IEntityTypeConfiguration<TKind>
    where TKind : Kind<TKind>
{
    public void Configure(EntityTypeBuilder<TKind> kind)
    {
        kind.HasStronglyTypedId();
        kind.HasListId();

        kind.HasIndex(nameof(Kind<TKind>.Name), Extensions.ListId).IsUnique();
        kind.ToTable(t => t.HasCheckConstraint($"CH_{tableName}_NameNotEmpty", "Name <> ''"));

        kind.Property(k => k.WatchedColor).HasConversion<ColorConverter>();
        kind.Property(k => k.NotWatchedColor).HasConversion<ColorConverter>();
        kind.Property(k => k.NotReleasedColor).HasConversion<ColorConverter>();
    }
}
