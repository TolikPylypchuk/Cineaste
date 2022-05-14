namespace Cineaste.Persistence.Configuration;

internal sealed class MovieTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> movie)
    {
        movie.HasStronglyTypedId();
        movie.HasTitles(m => m.Titles, "MovieTitles");
        movie.HasPoster(m => m.Poster);

        movie.HasCheckConstraint("CH_Movies_YearPositive", "Year > 0");

        movie.HasTags(m => m.Tags, "MovieTags");

        movie.Ignore(m => m.FranchiseItem);
    }
}
