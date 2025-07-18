namespace Cineaste.Persistence.Configuration;

internal sealed class FranchiseTypeConfiguration : IEntityTypeConfiguration<Franchise>
{
    public void Configure(EntityTypeBuilder<Franchise> franchise)
    {
        franchise.HasStronglyTypedId();
        franchise.HasTitles(f => f.Titles, "FranchiseTitles");
        franchise.HasPoster(f => f.Poster);

        franchise.HasMany(f => f.Children)
            .WithOne(item => item.ParentFranchise)
            .OnDelete(DeleteBehavior.Cascade);

        franchise.Navigation(f => f.Children)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        franchise.HasFranchiseItem(f => f.FranchiseItem, fi => fi.Franchise);

        franchise.Ignore(f => f.ActualTitles);
        franchise.Ignore(f => f.Title);
        franchise.Ignore(f => f.OriginalTitle);
        franchise.Ignore(f => f.ActualTitle);
        franchise.Ignore(f => f.ActualOriginalTitle);
    }
}
