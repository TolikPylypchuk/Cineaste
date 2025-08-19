namespace Cineaste.Persistence.Configuration;

internal sealed class FranchiseTypeConfiguration : IEntityTypeConfiguration<Franchise>
{
    public void Configure(EntityTypeBuilder<Franchise> franchise)
    {
        franchise.HasKey(f => f.Id);
        franchise.HasTitles(f => f.AllTitles, "FranchiseTitles");

        franchise.HasMany(f => f.Children)
            .WithOne(item => item.ParentFranchise)
            .OnDelete(DeleteBehavior.Cascade);

        franchise.Navigation(f => f.Children)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        franchise.HasFranchiseItem(f => f.FranchiseItem, fi => fi.Franchise);

        franchise.HasOne(f => f.MovieKind)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        franchise.HasOne(f => f.SeriesKind)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        franchise.Property(f => f.PosterHash)
            .IsFixedLength();

        franchise.Ignore(f => f.Titles);
        franchise.Ignore(f => f.OriginalTitles);
        franchise.Ignore(f => f.Title);
        franchise.Ignore(f => f.OriginalTitle);
    }
}
