namespace Cineaste.Persistence;

public class CineasteDbContext : DbContext
{
    public CineasteDbContext(DbContextOptions<CineasteDbContext> options)
        : base(options)
    { }

    public DbSet<Movie> Movies =>
        this.Set<Movie>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new MovieTypeConfiguration());
    }
}
