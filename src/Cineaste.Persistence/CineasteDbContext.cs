using Cineaste.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Cineaste.Persistence;

public class CineasteDbContext(DbContextOptions<CineasteDbContext> options)
    : IdentityDbContext<
        CineasteUser,
        CineasteRole,
        Id<CineasteUser>,
        CineasteUserClaim,
        CineasteUserRole,
        CineasteUserLogin,
        CineasteRoleClaim,
        CineasteUserToken
    >(options)
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

    public DbSet<CineasteUserInvitationCode> UserInvitationCodes =>
        this.Set<CineasteUserInvitationCode>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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

        builder.ApplyConfiguration(new CineasteUserInvitationCodeTypeConfiguration());

        builder.Entity<CineasteUser>(u => u.ToTable(nameof(this.Users)));
        builder.Entity<CineasteUserClaim>(u => u.ToTable(nameof(this.UserClaims)));
        builder.Entity<CineasteRole>(u => u.ToTable(nameof(this.Roles)));
        builder.Entity<CineasteRoleClaim>(u => u.ToTable(nameof(this.RoleClaims)));
        builder.Entity<CineasteUserRole>(u => u.ToTable(nameof(this.UserRoles)));
        builder.Entity<CineasteUserLogin>(u => u.ToTable(nameof(this.UserLogins)));
        builder.Entity<CineasteUserToken>(u => u.ToTable(nameof(this.UserTokens)));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        base.ConfigureConventions(builder);

        this.ConfigureIdConverters(builder);

        builder.Properties<Enum>()
            .HaveConversion<string>();

        builder.Properties<Color>()
            .HaveConversion<ColorConverter>();

        builder.Properties<ImdbId>()
            .HaveConversion<ImdbIdConverter>();

        builder.Properties<PosterHash>()
            .HaveConversion<PosterHashConverter>();

        builder.Properties<RottenTomatoesId>()
            .HaveConversion<RottenTomatoesIdConverter>();
    }

    private void ConfigureIdConverters(ModelConfigurationBuilder builder)
    {
        typeof(Entity<>)
            .Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && type.IsEntityType())
            .ForEach(type => builder
                .Properties(typeof(Id<>).MakeGenericType(type))
                .HaveConversion(typeof(IdConverter<>).MakeGenericType(type)));

        builder.Properties<Id<CineasteUser>>()
            .HaveConversion<IdConverter<CineasteUser>>();

        builder.Properties<Id<CineasteUserInvitationCode>>()
            .HaveConversion<IdConverter<CineasteUserInvitationCode>>();
    }
}

file static class Extensions
{
    public static bool IsEntityType(this Type type) =>
        type.BaseType is { } baseType &&
            (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Entity<>) ||
                baseType.IsEntityType());
}
