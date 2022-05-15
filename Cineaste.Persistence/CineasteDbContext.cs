namespace Cineaste.Persistence;

public class CineasteDbContext : DbContext
{
    public CineasteDbContext(DbContextOptions<CineasteDbContext> options)
        : base(options)
    { }

    public DbSet<Franchise> Franchises =>
        this.Set<Franchise>();

    public DbSet<FranchiseItem> FranchiseItems =>
        this.Set<FranchiseItem>();

    public DbSet<CineasteList> Lists =>
        this.Set<CineasteList>();

    public DbSet<ListConfiguration> ListConfigurations =>
        this.Set<ListConfiguration>();

    public DbSet<MovieKind> MovieKinds =>
        this.Set<MovieKind>();

    public DbSet<Movie> Movies =>
        this.Set<Movie>();

    public DbSet<Period> Periods =>
        this.Set<Period>();

    public DbSet<Season> Seasons =>
        this.Set<Season>();

    public DbSet<Series> Series =>
        this.Set<Series>();

    public DbSet<SeriesKind> SeriesKinds =>
        this.Set<SeriesKind>();

    public DbSet<SpecialEpisode> SpecialEpisodes =>
        this.Set<SpecialEpisode>();

    public DbSet<Tag> Tags =>
        this.Set<Tag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new KindTypeConfiguration<MovieKind>(nameof(MovieKinds)));
        builder.ApplyConfiguration(new KindTypeConfiguration<SeriesKind>(nameof(SeriesKinds)));
        builder.ApplyConfiguration(new TagTypeConfiguration());

        builder.ApplyConfiguration(new FranchiseTypeConfiguration());
        builder.ApplyConfiguration(new FranchiseItemTypeConfiguration());

        builder.ApplyConfiguration(new MovieTypeConfiguration());

        builder.ApplyConfiguration(new PeriodTypeConfiguration());
        builder.ApplyConfiguration(new SeasonTypeConfiguration());
        builder.ApplyConfiguration(new SpecialEpisodeTypeConfiguration());
        builder.ApplyConfiguration(new SeriesTypeConfiguration());

        builder.ApplyConfiguration(new ListConfigurationTypeConfiguration());
        builder.ApplyConfiguration(new CineasteListTypeConfiguration());
    }
}
