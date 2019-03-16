using Microsoft.EntityFrameworkCore.Design;

namespace MovieList.Data.Infrastructure
{
    internal class MovieContextFactory : IDesignTimeDbContextFactory<MovieContext>
    {
        public MovieContext CreateDbContext(string[] args)
            => new MovieContext("movies.db");
    }
}
