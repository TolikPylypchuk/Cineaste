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
    internal abstract class EntityServiceBase<TEntity, TTag> : ServiceBase, IEntityService<TEntity>
        where TEntity : EntityBase
        where TTag : EntityBase
    {
        private readonly IEqualityComparer<TTag> tagEqualityComparer;

        protected EntityServiceBase(string file, IEqualityComparer<TTag> tagEqualityComparer)
            : base(file)
            => this.tagEqualityComparer = tagEqualityComparer;

        public void Save(TEntity entity)
            => this.WithTransaction((connection, transaction) =>
            {
                if (entity.Id == default)
                {
                    this.Insert(entity, connection, transaction);
                } else
                {
                    this.Update(entity, connection, transaction);
                }

                this.UpdateTags(entity, connection, transaction);
            });

        public void Delete(TEntity entity)
            => this.WithTransaction((connection, transaction) =>
            {
                this.DeleteTags(entity, connection, transaction);
                this.Delete(entity, connection, transaction);
            });

        protected abstract void Insert(TEntity entity, IDbConnection connection, IDbTransaction transaction);
        protected abstract void Update(TEntity entity, IDbConnection connection, IDbTransaction transaction);
        protected abstract void Delete(TEntity entity, IDbConnection connection, IDbTransaction transaction);

        protected void DeleteFranchiseEntry(
            FranchiseEntry franchiseEntry,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            connection.Delete(franchiseEntry, transaction);

            var parentFranchise = franchiseEntry.ParentFranchise;

            foreach (var entry in parentFranchise.Entries
                .Where(entry => entry.SequenceNumber > franchiseEntry.SequenceNumber))
            {
                entry.SequenceNumber--;

                if (entry.DisplayNumber != null)
                {
                    entry.DisplayNumber--;
                }

                connection.Update(entry, transaction);
            }

            parentFranchise.Entries.Remove(franchiseEntry);

            if (parentFranchise.Entries.Count == 0)
            {
                connection.Delete(parentFranchise, transaction);

                if (parentFranchise.Entry != null)
                {
                    this.DeleteFranchiseEntry(parentFranchise.Entry, connection, transaction);
                }
            } else
            {
                this.UpdateMergedDisplayNumbers(franchiseEntry.Franchise ?? parentFranchise);
            }
        }

        protected void UpdateMergedDisplayNumbers(Franchise franchise)
        {
            if (franchise.Entry == null)
            {
                return;
            }

            int maxDisplayNumber = franchise.Entries.Select(entry => entry.DisplayNumber).Max() ?? 0;

            _ = franchise.Entry.ParentFranchise.Entries
                .OrderBy(entry => entry.SequenceNumber)
                .SkipWhile(entry => entry.SequenceNumber <= franchise.Entry.SequenceNumber)
                .TakeWhile(entry => entry.Franchise != null && entry.Franchise.MergeDisplayNumbers)
                .Aggregate(maxDisplayNumber + 1, this.UpdateMergedDisplayNumbers);
        }

        protected abstract List<TTag> GetTags(TEntity entity);

        private int UpdateMergedDisplayNumbers(int firstDisplayNumber, FranchiseEntry entry)
            => entry.Franchise!.Entries
                .OrderBy(e => e.SequenceNumber)
                .Aggregate(firstDisplayNumber, (num, e) =>
                {
                    if (e.DisplayNumber != null)
                    {
                        e.DisplayNumber = num;
                        return num + 1;
                    }

                    return num;
                });

        private void UpdateTags(TEntity entity, IDbConnection connection, IDbTransaction transaction)
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

        private void DeleteTags(TEntity entity, IDbConnection connection, IDbTransaction transaction)
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
