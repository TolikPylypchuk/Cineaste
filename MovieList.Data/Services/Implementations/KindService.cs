using System.Collections.Generic;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

using MovieList.Data.Models;

using Splat;

namespace MovieList.Data.Services.Implementations
{
    internal sealed class KindService : ServiceBase, IKindService, IEnableLogger
    {
        public KindService(string file)
            : base(file)
        { }

        [LogException]
        public async Task<IEnumerable<Kind>> GetAllKindsAsync()
        {
            this.Log().Debug("Getting all kinds.");

            await using var connection = this.GetSqliteConnection();
            await connection.OpenAsync();
            return await connection.GetAllAsync<Kind>();
        }
    }
}
