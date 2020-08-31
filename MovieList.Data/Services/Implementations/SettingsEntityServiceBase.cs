using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class SettingsEntityServiceBase<TEntity> : ServiceBase, ISettingsEntityService<TEntity>
        where TEntity : EntityBase
    {
        public SettingsEntityServiceBase(string file)
            : base(file)
        { }

        protected abstract string GetAllMessage { get; }
        protected abstract string UpdateAllMessage { get; }
        protected abstract string DeleteExceptionMessage { get; }

        public IEnumerable<TEntity> GetAll()
        {
            this.Log().Debug(this.GetAllMessage);
            return this.WithTransaction(this.GetAll);
        }

        public void UpdateAll(IEnumerable<TEntity> entities)
        {
            this.Log().Debug(this.UpdateAllMessage);
            this.WithTransaction((connection, transaction) => this.UpdateAll(entities, connection, transaction));
        }

        protected virtual IEnumerable<TEntity> GetAll(IDbConnection connection, IDbTransaction transaction)
            => connection.GetAll<TEntity>(transaction);

        protected virtual void UpdateAll(
            IEnumerable<TEntity> entities,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            var entityList = entities.ToList();

            var dbEntities = connection.GetAll<TEntity>(transaction).ToList();

            connection.Update(entityList.Intersect(dbEntities, IdEqualityComparer<TEntity>.Instance), transaction);

            foreach (var entityToInsert in entityList.Except(dbEntities, IdEqualityComparer<TEntity>.Instance))
            {
                entityToInsert.Id = (int)connection.Insert(entityToInsert, transaction);
            }

            var entitiesToDelete = dbEntities.Except(entityList, IdEqualityComparer<TEntity>.Instance).ToList();

            if (entitiesToDelete.Any(entity => !this.CanDelete(entity)))
            {
                throw new InvalidOperationException(this.DeleteExceptionMessage);
            }

            foreach (var entity in entitiesToDelete)
            {
                this.BeforeDelete(entity, connection, transaction);
            }

            connection.Delete(entitiesToDelete, transaction);
        }

        protected virtual void BeforeDelete(TEntity entity, IDbConnection connection, IDbTransaction transaction)
        { }

        protected abstract bool CanDelete(TEntity entity);
    }
}
