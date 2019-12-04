using System.Collections.Generic;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class KindService : ServiceBase, IKindService
    {
        public KindService(string file)
            : base(file)
        { }

        public async Task<IEnumerable<Kind>> GetAllKindsAsync()
        {
            this.Log().Debug("Getting all kinds.");

            return await this.WithTransactionAsync(
                (connection, transaction) => connection.GetAllAsync<Kind>(transaction));
        }
    }
}
