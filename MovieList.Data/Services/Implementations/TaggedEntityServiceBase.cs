using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

using Dapper;
using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class TaggedEntityServiceBase<TEntity, TTag> : EntityServiceBase<TEntity>
        where TEntity : EntityBase
        where TTag : EntityBase
    {
        private readonly IEqualityComparer<TTag> tagEqualityComparer;

        protected TaggedEntityServiceBase(string file, IEqualityComparer<TTag> tagEqualityComparer)
            : base(file)
            => this.tagEqualityComparer = tagEqualityComparer;

        protected abstract List<TTag> GetTags(TEntity entity);

        protected override void AfterSave(TEntity entity, IDbConnection connection, IDbTransaction transaction)
        {
            var tags = this.GetTags(entity);

            var (table, idColumn) = this.GetTagTableAndIdColumn();

            var dbTags = connection.Query<TTag>(
                $"SELECT * FROM {table} WHERE {idColumn} = @Id", new { entity.Id }, transaction);

            foreach (var tagToInsert in tags.Except(dbTags, this.tagEqualityComparer))
            {
                tagToInsert.Id = (int)connection.Insert(tagToInsert, transaction);
            }

            var tagsToDelete = dbTags.Except(tags, this.tagEqualityComparer).ToList();
            connection.Delete(tagsToDelete, transaction);
        }

        protected override void BeforeDelete(TEntity entity, IDbConnection connection, IDbTransaction transaction)
        {
            var (table, idColumn) = this.GetTagTableAndIdColumn();
            connection.Execute($"DELETE FROM {table} WHERE {idColumn} = @Id", new { entity.Id }, transaction);
        }

        private (string, string) GetTagTableAndIdColumn()
        {
            string table = typeof(TTag).GetCustomAttribute<TableAttribute>()?.Name
                ?? throw new InvalidOperationException($"The type {typeof(TTag)} doesn't have the Table attribute");

            string idColumn = $"{typeof(TEntity).Name}Id";

            return (table, idColumn);
        }
    }
}
