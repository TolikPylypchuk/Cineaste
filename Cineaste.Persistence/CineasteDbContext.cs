namespace Cineaste.Persistence;

public class CineasteDbContext : DbContext
{
    public CineasteDbContext(DbContextOptions<CineasteDbContext> options)
        : base(options)
    { }

    public DbSet<MovieKind> MovieKinds =>
        this.Set<MovieKind>();

    public DbSet<Movie> Movies =>
        this.Set<Movie>();

    public DbSet<SeriesKind> SeriesKinds =>
        this.Set<SeriesKind>();

    public DbSet<Tag> Tags =>
        this.Set<Tag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new KindTypeConfiguration<MovieKind>(nameof(MovieKinds)));
        builder.ApplyConfiguration(new KindTypeConfiguration<SeriesKind>(nameof(SeriesKinds)));
        builder.ApplyConfiguration(new MovieTypeConfiguration());
        builder.ApplyConfiguration(new TagTypeConfiguration());
    }
}
