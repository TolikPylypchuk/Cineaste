using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal abstract class ServiceBase : IEnableLogger
    {
        protected readonly string DatabasePath;

        protected ServiceBase(string file)
            => this.DatabasePath = file;

        protected async Task WithTransactionAsync(Func<DbConnection, IDbTransaction, Task> action)
        {
            await using var connection = this.GetConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await action(connection, transaction);
                await transaction.CommitAsync();
            } catch (Exception e)
            {
                await transaction.RollbackAsync();
                this.Log().Error(e);
                throw;
            }
        }

        protected async Task<TResult> WithTransactionAsync<TResult>(
            Func<DbConnection, IDbTransaction, Task<TResult>> action)
        {
            await using var connection = this.GetConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await action(connection, transaction);
                await transaction.CommitAsync();

                return result;
            } catch (Exception e)
            {
                await transaction.RollbackAsync();
                this.Log().Error(e);
                throw;
            }
        }

        private DbConnection GetConnection()
            => Locator.Current.GetService<DbConnection>(this.DatabasePath);
    }
}
