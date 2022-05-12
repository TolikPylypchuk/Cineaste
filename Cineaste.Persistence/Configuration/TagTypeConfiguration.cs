namespace Cineaste.Persistence.Configuration;

internal sealed class TagTypeConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> tag)
    {
        tag.HasStronglyTypedId();

        tag.HasIndex(t => new { t.Name, t.Category }).IsUnique();

        tag.Property(t => t.Category)
            .HasConversion(category => category.Name, name => new TagCategory(name));

        tag.HasCheckConstraint($"CH_Tag_NameNotEmpty", "Name <> ''");
        tag.HasCheckConstraint($"CH_Tag_CategoryNotEmpty", "Category <> ''");

        tag.Property(k => k.Color)
            .HasConversion<ColorConverter>();

        tag.HasMany(t => t.ImpliedTags)
            .WithMany(t => t.ImplyingTags)
            .UsingEntity<Dictionary<string, object>>(
                "Cineaste.Core.Domain.TagImplication",
                i => i.HasOne<Tag>().WithMany().HasForeignKey("ImpliedTagId"),
                i => i.HasOne<Tag>().WithMany().HasForeignKey("ImplyingTagId"),
                i => i.ToTable("TagImplications"));

        tag.Navigation(t => t.ImpliedTags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        tag.Navigation(t => t.ImplyingTags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
