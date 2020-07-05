using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal class ListService : ServiceBase, IListService
    {
        public ListService(string file)
            : base(file)
        { }

        public MovieList GetList(IList<Kind> kinds)
            => this.WithTransaction((connection, transaction) => this.GetList(kinds, connection, transaction));

        private MovieList GetList(
            IList<Kind> kinds,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            this.Log().Debug("Getting the full list of movies, series and franchise");

            var titles = connection.GetAll<Title>(transaction).ToList();
            var entries = connection.GetAll<FranchiseEntry>(transaction).ToList();

            var seasons = connection.GetAll<Season>(transaction).ToList();
            var periods = connection.GetAll<Period>(transaction).ToList();
            var specialEpisodes = connection.GetAll<SpecialEpisode>(transaction).ToList();

            var movies = connection.GetAll<Movie>(transaction).ToList();
            var series = connection.GetAll<Series>(transaction).ToList();
            var franchise = connection.GetAll<Franchise>(transaction).ToList();

            return new MovieList(
                movies
                    .Join(kinds)
                    .Join(titles)
                    .Join(entries)
                    .ToList(),
                series
                    .Join(kinds)
                    .Join(titles)
                    .Join(seasons.Join(periods).Join(titles))
                    .Join(specialEpisodes.Join(titles))
                    .Join(entries)
                    .ToList(),
                franchise
                    .Join(titles)
                    .Join(entries)
                    .ToList());
        }
    }
}
