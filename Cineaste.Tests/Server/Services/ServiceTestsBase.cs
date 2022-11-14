namespace Cineaste.Server.Services;

using System.Runtime.CompilerServices;

using Cineaste.Persistence;

using Microsoft.EntityFrameworkCore;

public abstract class ServiceTestsBase
{
    protected CineasteDbContext CreateInMemoryDb([CallerMemberName] string dbName = "")
    {
        var options = new DbContextOptionsBuilder<CineasteDbContext>()
            .UseInMemoryDatabase(databaseName: this.GetType().Name + "." + dbName)
            .Options;

        var context = new CineasteDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
