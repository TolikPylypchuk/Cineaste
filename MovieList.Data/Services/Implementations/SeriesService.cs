using System;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class SeriesService : EntityServiceBase<Series>
    {
        public SeriesService(string fileName)
            : base(fileName)
        { }

        public override Task SaveAsync(Series series)
        {
            throw new NotImplementedException();
        }

        public override async Task DeleteAsync(Series series)
        {
            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            await connection.DeleteAsync(series.Titles, transaction);

            await connection.DeleteAsync(series.Seasons.SelectMany(season => season.Periods), transaction);
            await connection.DeleteAsync(series.Seasons.SelectMany(season => season.Titles), transaction);
            await connection.DeleteAsync(series.Seasons, transaction);

            await connection.DeleteAsync(series.SpecialEpisodes.SelectMany(episode => episode.Titles), transaction);
            await connection.DeleteAsync(series.SpecialEpisodes, transaction);

            if (series.Entry != null)
            {
                await this.DeleteMovieSeriesEntryAsync(series.Entry, connection, transaction);
            }

            await connection.DeleteAsync(series, transaction);

            await transaction.CommitAsync();
        }
    }
}
