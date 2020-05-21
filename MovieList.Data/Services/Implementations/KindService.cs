using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class KindService : ServiceBase, IKindService
    {
        public KindService(string file)
            : base(file)
        { }

        public async Task<IEnumerable<Kind>> GetAllKindsAsync()
        {
            this.Log().Debug("Getting all kinds");

            return await this.WithTransactionAsync(
                (connection, transaction) => connection.GetAllAsync<Kind>(transaction));
        }

        public Task UpdateKindsAsync(IEnumerable<Kind> kinds)
            => this.WithTransactionAsync(async (connection, transaction) =>
            {
                this.Log().Debug("Updating kinds");

                var kindsList = kinds.ToList();

                var dbKinds = (await connection.GetAllAsync<Kind>(transaction)).ToList();

                await connection.UpdateAsync(
                    kindsList.Intersect(dbKinds, IdEqualityComparer<Kind>.Instance), transaction);

                foreach (var kindToInsert in kindsList.Except(dbKinds, IdEqualityComparer<Kind>.Instance))
                {
                    kindToInsert.Id = await connection.InsertAsync(kindToInsert, transaction);
                }

                var kindsToDelete = dbKinds.Except(kindsList, IdEqualityComparer<Kind>.Instance).ToList();

                if (kindsToDelete.Any(kind => kind.Movies.Count != 0 || kind.Series.Count != 0))
                {
                    throw new InvalidOperationException(
                        "Cannot delete kinds that have movies or series attached to them");
                }

                await connection.DeleteAsync(kindsToDelete, transaction);
            });
    }
}
