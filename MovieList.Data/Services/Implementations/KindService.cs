using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Kind> GetAllKinds()
        {
            this.Log().Debug("Getting all kinds");
            return this.WithTransaction((connection, transaction) => connection.GetAll<Kind>(transaction));
        }

        public void UpdateKinds(IEnumerable<Kind> kinds)
            => this.WithTransaction((connection, transaction) =>
            {
                this.Log().Debug("Updating kinds");

                var kindsList = kinds.ToList();

                var dbKinds = connection.GetAll<Kind>(transaction).ToList();

                connection.Update(kindsList.Intersect(dbKinds, IdEqualityComparer<Kind>.Instance), transaction);

                foreach (var kindToInsert in kindsList.Except(dbKinds, IdEqualityComparer<Kind>.Instance))
                {
                    kindToInsert.Id = (int)connection.Insert(kindToInsert, transaction);
                }

                var kindsToDelete = dbKinds.Except(kindsList, IdEqualityComparer<Kind>.Instance).ToList();

                if (kindsToDelete.Any(kind => kind.Movies.Count != 0 || kind.Series.Count != 0))
                {
                    throw new InvalidOperationException(
                        "Cannot delete kinds that have movies or series attached to them");
                }

                connection.Delete(kindsToDelete, transaction);
            });
    }
}
