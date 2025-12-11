using Cineaste.Application.Services.Poster;

using Microsoft.EntityFrameworkCore;

using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(DbFixture))]

namespace Cineaste.Application.Fixture;

public sealed class DbFixture : IAsyncLifetime
{
    private readonly MsSqlContainer container = new MsSqlBuilder()
        .WithReuse(true)
        .WithLabel("reuse-id", "DB")
        .Build();

    public async ValueTask InitializeAsync()
    {
        await this.container.StartAsync();

        var dbContext = this.CreateDbContext();
        await dbContext.Database.MigrateAsync(TestContext.Current.CancellationToken);

        dbContext.Tags.RemoveRange(dbContext.Tags);
        dbContext.FranchiseItems.RemoveRange(dbContext.FranchiseItems);
        dbContext.Franchises.RemoveRange(dbContext.Franchises);
        dbContext.Movies.RemoveRange(dbContext.Movies);
        dbContext.Periods.RemoveRange(dbContext.Periods);
        dbContext.Seasons.RemoveRange(dbContext.Seasons);
        dbContext.SpecialEpisodes.RemoveRange(dbContext.SpecialEpisodes);
        dbContext.Series.RemoveRange(dbContext.Series);
        dbContext.MovieKinds.RemoveRange(dbContext.MovieKinds);
        dbContext.SeriesKinds.RemoveRange(dbContext.SeriesKinds);
        dbContext.ListConfigurations.RemoveRange(dbContext.ListConfigurations);
        dbContext.ListItems.RemoveRange(dbContext.ListItems);
        dbContext.Lists.RemoveRange(dbContext.Lists);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public ValueTask DisposeAsync() =>
        this.container.DisposeAsync();

    public DataFixture CreateDataFixture() =>
        new(this);

    public CineasteDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CineasteDbContext>()
            .UseSqlServer(this.container.GetConnectionString())
            .Options;

        return new CineasteDbContext(options);
    }
}
