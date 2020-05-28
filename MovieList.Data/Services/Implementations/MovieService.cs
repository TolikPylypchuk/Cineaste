using System.Data;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class MovieService : EntityServiceBase<Movie>
    {
        public MovieService(string fileName)
            : base(fileName)
        { }

        protected override void Insert(Movie movie, IDbConnection connection, IDbTransaction transaction)
        {
            movie.KindId = movie.Kind.Id;

            movie.Id = (int)connection.Insert(movie, transaction);

            foreach (var title in movie.Titles)
            {
                title.MovieId = movie.Id;
                title.Id = (int)connection.Insert(title, transaction);
            }

            if (movie.Entry != null)
            {
                var entry = movie.Entry;
                entry.MovieId = movie.Id;
                entry.ParentSeriesId = entry.ParentSeries.Id;
                entry.Id = (int)connection.Insert(entry, transaction);
                entry.ParentSeries.Entries.Add(entry);

                this.UpdateMergedDisplayNumbers(entry.ParentSeries);
            }
        }

        protected override void Update(Movie movie, IDbConnection connection, IDbTransaction transaction)
        {
            connection.Update(movie, transaction);

            new DependentEntityUpdater(connection, transaction).UpdateDependentEntities(
                movie,
                movie.Titles,
                title => title.MovieId,
                (title, movieId) => title.MovieId = movieId);

            if (movie.Entry != null)
            {
                connection.Update(movie.Entry, transaction);
            }
        }

        protected override void Delete(Movie movie, IDbConnection connection, IDbTransaction transaction)
        {
            connection.Delete(movie.Titles, transaction);

            if (movie.Entry != null)
            {
                this.DeleteMovieSeriesEntry(movie.Entry, connection, transaction);
            }

            connection.Delete(movie, transaction);
        }
    }
}
