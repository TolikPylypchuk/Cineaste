using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MovieList.Data.Infrastructure
{
    internal class MovieContextFactory : IDesignTimeDbContextFactory<MovieContext>
    {
        public MovieContext CreateDbContext(string[] args)
            => new MovieContext(new DbContextOptionsBuilder<MovieContext>().UseSqlite("Data Source=movies.db").Options);
    }
}
