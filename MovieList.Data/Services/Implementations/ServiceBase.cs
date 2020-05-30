using System;
using System.Data;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class ServiceBase : IEnableLogger
    {
        protected readonly string DatabasePath;

        protected ServiceBase(string file)
            => this.DatabasePath = file;

        protected void WithTransaction(Action<IDbConnection, IDbTransaction> action)
        {
            using var connection = this.OpenConnection();
            using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                action(connection, transaction);
                transaction.Commit();
            } catch (Exception e)
            {
                transaction.Rollback();
                this.Log().Error(e);
                throw;
            } finally
            {
                connection.Close();
            }
        }

        protected TResult WithTransaction<TResult>(Func<IDbConnection, IDbTransaction, TResult> action)
        {
            using var connection = this.OpenConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var result = action(connection, transaction);
                transaction.Commit();

                return result;
            } catch (Exception e)
            {
                transaction.Rollback();
                this.Log().Error(e);
                throw;
            } finally
            {
                connection.Close();
            }
        }

        private IDbConnection OpenConnection()
        {
            var connection = Locator.Current.GetService<IDbConnection>(this.DatabasePath);
            connection.Open();

            var walCommand = connection.CreateCommand();
            walCommand.CommandText = @"PRAGMA journal_mode = 'wal'";
            walCommand.ExecuteNonQuery();

            return connection;
        }
    }
}
