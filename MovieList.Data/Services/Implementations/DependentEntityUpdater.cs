using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class DependentEntityUpdater
    {
        private readonly DbConnection connection;
        private readonly IDbTransaction transaction;

        public DependentEntityUpdater(DbConnection connection, IDbTransaction transaction)
        {
            this.connection = connection;
            this.transaction = transaction;
        }

        public async Task UpdateDependentEntitiesAsync<TDependent>(
            EntityBase entity,
            IList<TDependent> dependentEntities,
            Func<TDependent, int?> relationKeyGetter,
            Action<TDependent, int> relationKeySetter,
            Func<TDependent, Task>? entityToInsertCallback = null,
            Func<TDependent, Task>? entityToDeleteCallback = null)
            where TDependent : EntityBase
        {
            var dbEntities = (await this.connection.GetAllAsync<TDependent>(this.transaction))
                .Where(de => relationKeyGetter(de) == entity.Id)
                .ToList();

            await this.connection.UpdateAsync(
                dependentEntities.Intersect(dbEntities, IdEqualityComparer<TDependent>.Instance), this.transaction);

            foreach (var entityToInsert in dependentEntities.Except(
                dbEntities, IdEqualityComparer<TDependent>.Instance))
            {
                relationKeySetter(entityToInsert, entity.Id);
                entityToInsert.Id = await this.connection.InsertAsync(entityToInsert, this.transaction);

                if (entityToInsertCallback != null)
                {
                    await entityToInsertCallback(entityToInsert);
                }
            }

            var entitiesToDelete = dbEntities.Except(dependentEntities, IdEqualityComparer<TDependent>.Instance)
                .ToList();

            if (entityToDeleteCallback != null)
            {
                foreach (var entityToDelete in entitiesToDelete)
                {
                    await entityToDeleteCallback(entityToDelete);
                }
            }

            await this.connection.DeleteAsync(entitiesToDelete, this.transaction);
        }
    }
}
