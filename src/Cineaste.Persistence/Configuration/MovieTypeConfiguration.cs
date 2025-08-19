namespace Cineaste.Persistence.Configuration;

internal sealed class MovieTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> movie)
    {
        movie.HasKey(m => m.Id);

        movie.HasTitles(m => m.AllTitles, "MovieTitles");

        movie.ToTable(t => t.HasCheckConstraint("CH_Movies_YearPositive", "[Year] > 0"));

        movie.HasOne(m => m.Kind)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        movie.Property(m => m.PosterHash)
            .IsFixedLength();

        movie.HasTags(m => m.Tags, "MovieTags");
        movie.HasFranchiseItem(m => m.FranchiseItem, fi => fi.Movie);

        movie.Ignore(m => m.Titles);
        movie.Ignore(m => m.OriginalTitles);
        movie.Ignore(m => m.Title);
        movie.Ignore(m => m.OriginalTitle);
    }
}
