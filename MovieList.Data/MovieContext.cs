using Microsoft.EntityFrameworkCore;

using MovieList.Data.Models;

namespace MovieList.Data
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options)
               : base(options)
        { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieSeries> MovieSeries { get; set; }
        public DbSet<MovieSeriesEntry> MovieSeriesEntries { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Series> Seasons { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<SpecialEpisode> SpecialEpisodes { get; set; }
        public DbSet<Kind> Titles { get; set; }
        public DbSet<Kind> Kinds { get; set; }
    }
}
