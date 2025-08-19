namespace Cineaste.Persistence.Configuration;

internal sealed class KindTypeConfiguration<TKind>(string tableName) : IEntityTypeConfiguration<TKind>
    where TKind : Kind<TKind>
{
    public void Configure(EntityTypeBuilder<TKind> kind)
    {
        kind.HasKey(k => k.Id);
        kind.HasListId();

        kind.HasIndex(nameof(Kind<TKind>.Name), Extensions.ListId).IsUnique();
        kind.ToTable(t => t.HasCheckConstraint($"CH_{tableName}_NameNotEmpty", "[Name] <> ''"));
    }
}
