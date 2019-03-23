using Microsoft.EntityFrameworkCore;

namespace MovieList.Data
{
    public static class BuilderExtensions
    {
        public static DbContextOptionsBuilder ConfigureMovieContext(this DbContextOptionsBuilder builder, string dbPath)
            => builder.UseLazyLoadingProxies().UseSqlite($"Data Source={dbPath}");
    }
}
