using System.Data;
using System.Linq;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class EntityServiceBase<TEntity> : ServiceBase, IEntityService<TEntity>
        where TEntity : EntityBase
    {
        protected EntityServiceBase(string file)
            : base(file)
        { }

        public void Save(TEntity entity) =>
            this.WithTransaction((connection, transaction) =>
            {
                if (entity.Id == default)
                {
                    this.Insert(entity, connection, transaction);
                } else
                {
                    this.Update(entity, connection, transaction);
                }

                this.AfterSave(entity, connection, transaction);
            });

        public void Delete(TEntity entity) =>
            this.WithTransaction((connection, transaction) =>
            {
                this.BeforeDelete(entity, connection, transaction);
                this.Delete(entity, connection, transaction);
            });

        protected abstract void Insert(TEntity entity, IDbConnection connection, IDbTransaction transaction);
        protected abstract void Update(TEntity entity, IDbConnection connection, IDbTransaction transaction);
        protected abstract void Delete(TEntity entity, IDbConnection connection, IDbTransaction transaction);

        protected virtual void AfterSave(TEntity entity, IDbConnection connection, IDbTransaction transaction)
        { }

        protected virtual void BeforeDelete(TEntity entity, IDbConnection connection, IDbTransaction transaction)
        { }

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

        private int UpdateMergedDisplayNumbers(int firstDisplayNumber, FranchiseEntry entry) =>
            entry.Franchise!.Entries
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
    }
}
