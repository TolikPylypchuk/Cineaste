namespace Cineaste.Persistence.Configuration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal static class Extensions
{
    public static void HasStronglyTypedId<T>(this EntityTypeBuilder<T> builder)
        where T : Entity<T>
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, guid => new Id<T>(guid));
    }
}
