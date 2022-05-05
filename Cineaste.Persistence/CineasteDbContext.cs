namespace Cineaste.Persistence;

public class CineasteDbContext : DbContext
{
    public CineasteDbContext(DbContextOptions<CineasteDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Movie>(movie =>
        {
            movie.HasStronglyTypedId();
            movie.HasCheckConstraint("CH_Movie_YearPositive", "Year > 0");

            movie.Ignore(m => m.Poster);
            movie.Ignore(m => m.Titles);
            movie.Ignore(m => m.OwnerList);
            movie.Ignore(m => m.Kind);
            movie.Ignore(m => m.Tags);
            movie.Ignore(m => m.FranchiseItem);
        });
    }
}
