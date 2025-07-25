namespace Cineaste.Persistence.Configuration;

internal sealed class MovieTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> movie)
    {
        movie.HasStronglyTypedId();

        movie.HasTitles(m => m.AllTitles, "MovieTitles");
        movie.HasPoster(m => m.Poster);

        movie.ToTable(t => t.HasCheckConstraint("CH_Movies_YearPositive", "Year > 0"));

        movie.HasOne(m => m.Kind)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        movie.HasTags(m => m.Tags, "MovieTags");
        movie.HasFranchiseItem(m => m.FranchiseItem, fi => fi.Movie);

        movie.Ignore(m => m.Titles);
        movie.Ignore(m => m.OriginalTitles);
        movie.Ignore(m => m.Title);
        movie.Ignore(m => m.OriginalTitle);
    }
}
