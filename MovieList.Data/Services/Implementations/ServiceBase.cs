using System;
using System.Data;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class ServiceBase : IEnableLogger
    {
        protected readonly string DatabasePath;

        protected ServiceBase(string file)
            => this.DatabasePath = file;

        [LogException]
        protected async Task WithTransactionAsync(Func<SqliteConnection, IDbTransaction, Task> action)
        {
            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await action(connection, transaction);
                await transaction.CommitAsync();
            } catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [LogException]
        protected async Task<TResult> WithTransactionAsync<TResult>(
            Func<SqliteConnection, IDbTransaction, Task<TResult>> action)
        {
            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await action(connection, transaction);
                await transaction.CommitAsync();

                return result;
            } catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private SqliteConnection GetSqliteConnection()
            => Locator.Current.GetService<SqliteConnection>(this.DatabasePath);
    }
}
