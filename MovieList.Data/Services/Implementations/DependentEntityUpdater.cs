using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class DependentEntityUpdater
    {
        private readonly IDbConnection connection;
        private readonly IDbTransaction transaction;

        public DependentEntityUpdater(IDbConnection connection, IDbTransaction transaction)
        {
            this.connection = connection;
            this.transaction = transaction;
        }

        public void UpdateDependentEntities<TDependent>(
            EntityBase entity,
            IList<TDependent> dependentEntities,
            Func<TDependent, int?> relationKeyGetter,
            Action<TDependent, int> relationKeySetter,
            Action<TDependent>? entityToInsertCallback = null,
            Action<TDependent>? entityToDeleteCallback = null)
            where TDependent : EntityBase
        {
            var dbEntities = this.connection.GetAll<TDependent>(this.transaction)
                .Where(de => relationKeyGetter(de) == entity.Id)
                .ToList();

            this.connection.Update(
                dependentEntities.Intersect(dbEntities, IdEqualityComparer<TDependent>.Instance), this.transaction);

            foreach (var entityToInsert in dependentEntities.Except(
                dbEntities, IdEqualityComparer<TDependent>.Instance))
            {
                relationKeySetter(entityToInsert, entity.Id);
                entityToInsert.Id = (int)this.connection.Insert(entityToInsert, this.transaction);

                entityToInsertCallback?.Invoke(entityToInsert);
            }

            var entitiesToDelete = dbEntities
                .Except(dependentEntities, IdEqualityComparer<TDependent>.Instance)
                .ToList();

            if (entityToDeleteCallback != null)
            {
                entitiesToDelete.ForEach(entityToDeleteCallback);
            }

            this.connection.Delete(entitiesToDelete, this.transaction);
        }
    }
}
