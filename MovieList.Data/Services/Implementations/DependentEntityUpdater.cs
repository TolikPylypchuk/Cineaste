using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class DependentEntityUpdater
    {
        private readonly SqliteConnection connection;
        private readonly IDbTransaction transaction;

        public DependentEntityUpdater(SqliteConnection connection, IDbTransaction transaction)
        {
            this.connection = connection;
            this.transaction = transaction;
        }

        public async Task UpdateDependentEntitiesAsync<TDependent>(
            EntityBase entity,
            IList<TDependent> dependentEntities,
            Func<TDependent, int?> relationKeyGetter,
            Action<TDependent, int> relationKeySetter)
            where TDependent : EntityBase
        {
            var dbEntities = (await this.connection.GetAllAsync<TDependent>(this.transaction))
                .Where(de => relationKeyGetter(de) == entity.Id)
                .ToList();

            await this.connection.UpdateAsync(
                dependentEntities.Intersect(dbEntities, IdEqualityComparer<TDependent>.Instance), this.transaction);

            foreach (var dependentEntity in dependentEntities.Except(
                dbEntities, IdEqualityComparer<TDependent>.Instance))
            {
                relationKeySetter(dependentEntity, entity.Id);
                dependentEntity.Id = await this.connection.InsertAsync(dependentEntity, this.transaction);
            }

            await this.connection.DeleteAsync(
                dbEntities.Except(dependentEntities, IdEqualityComparer<TDependent>.Instance), this.transaction);
        }
    }
}
