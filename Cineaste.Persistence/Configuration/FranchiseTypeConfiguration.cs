namespace Cineaste.Persistence.Configuration;

internal sealed class FranchiseTypeConfiguration : IEntityTypeConfiguration<Franchise>
{
    public void Configure(EntityTypeBuilder<Franchise> franchise)
    {
        franchise.HasStronglyTypedId();
        franchise.HasListId();
        franchise.HasTitles(f => f.Titles, "FranchiseTitles");
        franchise.HasPoster(f => f.Poster);

        franchise.HasMany(f => f.Children)
            .WithOne(item => item.ParentFranchise)
            .OnDelete(DeleteBehavior.Restrict);

        franchise.Navigation(f => f.Children)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        franchise.HasFranchiseItem(f => f.FranchiseItem);
    }
}
