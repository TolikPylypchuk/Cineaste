namespace Cineaste.Persistence;

public class CineasteDbContext(DbContextOptions<CineasteDbContext> options) : DbContext(options)
{
    public DbSet<Franchise> Franchises =>
        this.Set<Franchise>();

    public DbSet<FranchiseItem> FranchiseItems =>
        this.Set<FranchiseItem>();

    public DbSet<FranchisePoster> FranchisePosters =>
        this.Set<FranchisePoster>();

    public DbSet<CineasteList> Lists =>
        this.Set<CineasteList>();

    public DbSet<ListConfiguration> ListConfigurations =>
        this.Set<ListConfiguration>();

    public DbSet<ListItem> ListItems =>
        this.Set<ListItem>();

    public DbSet<MovieKind> MovieKinds =>
        this.Set<MovieKind>();

    public DbSet<Movie> Movies =>
        this.Set<Movie>();

    public DbSet<MoviePoster> MoviePosters =>
        this.Set<MoviePoster>();

    public DbSet<Period> Periods =>
        this.Set<Period>();

    public DbSet<Season> Seasons =>
        this.Set<Season>();

    public DbSet<SeasonPoster> SeasonPosters =>
        this.Set<SeasonPoster>();

    public DbSet<Series> Series =>
        this.Set<Series>();

    public DbSet<SeriesPoster> SeriesPosters =>
        this.Set<SeriesPoster>();

    public DbSet<SeriesKind> SeriesKinds =>
        this.Set<SeriesKind>();

    public DbSet<SpecialEpisode> SpecialEpisodes =>
        this.Set<SpecialEpisode>();

    public DbSet<SpecialEpisodePoster> SpecialEpisodePosters =>
        this.Set<SpecialEpisodePoster>();

    public DbSet<Tag> Tags =>
        this.Set<Tag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new KindTypeConfiguration<MovieKind>(nameof(this.MovieKinds)));
        builder.ApplyConfiguration(new KindTypeConfiguration<SeriesKind>(nameof(this.SeriesKinds)));
        builder.ApplyConfiguration(new TagTypeConfiguration());

        builder.ApplyConfiguration(new FranchiseTypeConfiguration());
        builder.ApplyConfiguration(new FranchiseItemTypeConfiguration());
        builder.ApplyConfiguration(new PosterTypeConfiguration<FranchisePoster>(nameof(this.FranchisePosters)));

        builder.ApplyConfiguration(new MovieTypeConfiguration());
        builder.ApplyConfiguration(new PosterTypeConfiguration<MoviePoster>(nameof(this.MoviePosters)));

        builder.ApplyConfiguration(new PeriodTypeConfiguration());
        builder.ApplyConfiguration(new SeasonTypeConfiguration());
        builder.ApplyConfiguration(new PosterTypeConfiguration<SeasonPoster>(nameof(this.SeasonPosters)));

        builder.ApplyConfiguration(new SpecialEpisodeTypeConfiguration());
        builder.ApplyConfiguration(new PosterTypeConfiguration<SpecialEpisodePoster>(
            nameof(this.SpecialEpisodePosters)));

        builder.ApplyConfiguration(new SeriesTypeConfiguration());
        builder.ApplyConfiguration(new PosterTypeConfiguration<SeriesPoster>(nameof(this.SeriesPosters)));

        builder.ApplyConfiguration(new ListConfigurationTypeConfiguration());
        builder.ApplyConfiguration(new CineasteListTypeConfiguration());
        builder.ApplyConfiguration(new ListItemTypeConfiguration());
    }
}
