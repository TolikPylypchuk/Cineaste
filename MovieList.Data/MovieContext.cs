using Microsoft.EntityFrameworkCore;

using MovieList.Data.Models;

#pragma warning disable CS8618 // Non-nullable field is uninitialized.

namespace MovieList.Data
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options)
               : base(options)
        { }

        public MovieContext(string path)
            : this(new DbContextOptionsBuilder<MovieContext>().UseSqlite($"Data Source={path}").Options)
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

#pragma warning restore CS8618 // Non-nullable field is uninitialized.
