namespace Cineaste.Persistence.Configuration;

internal sealed class PosterTypeConfiguration<TPoster>(string tableName) : IEntityTypeConfiguration<TPoster>
    where TPoster : Poster<TPoster>
{
    public void Configure(EntityTypeBuilder<TPoster> poster)
    {
        poster.HasStronglyTypedId();

        poster.ToTable(t => t.HasCheckConstraint($"CH_{tableName}_DataNotEmpty", "DATALENGTH(Data) > 0"));
        poster.ToTable(t => t.HasCheckConstraint($"CH_{tableName}_ContentTypeNotEmpty", "ContentType <> ''"));
    }
}
