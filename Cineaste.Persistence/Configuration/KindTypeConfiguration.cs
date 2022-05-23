namespace Cineaste.Persistence.Configuration;

internal sealed class KindTypeConfiguration<TKind> : IEntityTypeConfiguration<TKind>
    where TKind : Kind<TKind>
{
    private readonly string tableName;

    public KindTypeConfiguration(string tableName) =>
        this.tableName = tableName;

    public void Configure(EntityTypeBuilder<TKind> kind)
    {
        kind.HasStronglyTypedId();
        kind.HasListId();

        kind.HasIndex(nameof(Kind<TKind>.Name), Extensions.ListId).IsUnique();
        kind.HasCheckConstraint($"CH_{tableName}_NameNotEmpty", "Name <> ''");

        kind.Property(k => k.WatchedColor).HasConversion<ColorConverter>();
        kind.Property(k => k.NotWatchedColor).HasConversion<ColorConverter>();
        kind.Property(k => k.NotReleasedColor).HasConversion<ColorConverter>();
    }
}
